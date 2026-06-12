using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEaseBookingSystem.Data;
using EventEaseBookingSystem.Models;
using System.Linq;
using System.Threading.Tasks;
using EventEaseBookingSystem.Services;

namespace EventEaseBookingSystem.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventEaseContext _context;

        private readonly BlobService _blobService;

        public EventsController(EventEaseContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: Events
        // GET: Events
        public async Task<IActionResult> Index(
     string? eventType,
     DateTime? startDate,
     DateTime? endDate,
     bool? venueAvailable)
        {
            var events = _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .AsQueryable();

            // Filter by Event Type
            if (!string.IsNullOrEmpty(eventType))
            {
                events = events.Where(e => e.EventType.EventTypeName == eventType);
            }

            // Filter by Start Date
            if (startDate.HasValue)
            {
                events = events.Where(e => e.EventDate >= startDate.Value);
            }

            // Filter by End Date
            if (endDate.HasValue)
            {
                events = events.Where(e => e.EventDate <= endDate.Value);
            }

            // Filter by Venue Availability
            if (venueAvailable.HasValue)
            {
                events = events.Where(e => e.Venue.IsAvailable == venueAvailable.Value);
            }

            ViewBag.EventTypes = _context.EventTypes.ToList();

            ViewBag.SelectedEventType = eventType;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.SelectedVenueAvailable = venueAvailable;

            return View(await events.ToListAsync());
        }
        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            if (!_context.Venues.Any())
            {
                TempData["Error"] = "Please create a venue first.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.VenueId = new SelectList(_context.Venues, "VenueId", "VenueName");
            ViewBag.EventTypeId = new SelectList(_context.EventTypes, "EventTypeId", "EventTypeName");

            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                // Check if venue already has event
                bool venueBooked = _context.Events.Any(e =>
                    e.VenueId == @event.VenueId &&
                    e.EventDate == @event.EventDate);

                if (venueBooked)
                {
                    ModelState.AddModelError("",
                        "This venue is already booked for the selected date and time.");

                    ViewBag.VenueId = new SelectList(
                        _context.Venues,
                        "VenueId",
                        "VenueName",
                        @event.VenueId);

                    ViewBag.EventTypeId = new SelectList(
                         _context.EventTypes,
                         "EventTypeId",
                         "EventTypeName",
                         @event.EventTypeId);

                    return View(@event);
                }

                // Upload image
                if (@event.ImageFile != null)
                {
                    @event.ImageUrl =
                        await _blobService.UploadFileAsync(@event.ImageFile);
                }

                _context.Events.Add(@event);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.VenueId = new SelectList(
                _context.Venues,
                "VenueId",
                "VenueName",
                @event.VenueId);

            ViewBag.EventTypeId = new SelectList(
                _context.EventTypes,
                "EventTypeId",
                "EventTypeName",
                @event.EventTypeId);

            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            ViewBag.VenueId = new SelectList(
               _context.Venues,
              "VenueId",
              "VenueName",
              @event.VenueId);

            ViewBag.EventTypeId = new SelectList(
                _context.EventTypes,
                "EventTypeId",
                "EventTypeName",
                @event.EventTypeId);

            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event @event)
        {
            if (id != @event.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingEvent =
                    await _context.Events.FindAsync(id);

                if (existingEvent == null)
                {
                    return NotFound();
                }

                // Update normal fields
                existingEvent.EventName = @event.EventName;
                existingEvent.EventDate = @event.EventDate;
                existingEvent.Description = @event.Description;
                existingEvent.VenueId = @event.VenueId;
                existingEvent.EventTypeId = @event.EventTypeId;

                // Upload NEW image if selected
                if (@event.ImageFile != null)
                {
                    existingEvent.ImageUrl =
                        await _blobService.UploadFileAsync(@event.ImageFile);
                }

                _context.Update(existingEvent);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.VenueId = new SelectList(
                _context.Venues,
                "VenueId",
                "VenueName",
                @event.VenueId);

            ViewBag.EventTypeId = new SelectList(
                _context.EventTypes,
                "EventTypeId",
                "EventTypeName",
                @event.EventTypeId);

            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events
                .Include(e => e.Bookings)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (@event == null)
            {
                return NotFound();
            }

            // Check if event has bookings
            if (@event.Bookings != null && @event.Bookings.Any())
            {
                TempData["ErrorMessage"] =
                    "Cannot delete event because it has active bookings.";

                return RedirectToAction(nameof(Index));
            }

            _context.Events.Remove(@event);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
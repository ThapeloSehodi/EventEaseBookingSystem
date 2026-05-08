using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEaseBookingSystem.Data;
using EventEaseBookingSystem.Models;
using EventEaseBookingSystem.Services;

namespace EventEaseBookingSystem.Controllers
{
    public class VenuesController : Controller
    {
        private readonly EventEaseContext _context;

        private readonly BlobService _blobService;

        public VenuesController(EventEaseContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Venues.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id);

            if (venue == null) return NotFound();

            return View(venue);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue)
        {
            if (ModelState.IsValid)
            {
                // Upload image to Azure Blob Storage
                if (venue.ImageFile != null)
                {
                    venue.ImageUrl = await _blobService.UploadFileAsync(venue.ImageFile);
                }

                _context.Add(venue);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FindAsync(id);

            if (venue == null) return NotFound();

            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue)
        {
            if (id != venue.VenueId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingVenue = await _context.Venues.FindAsync(id);

                if (existingVenue == null)
                {
                    return NotFound();
                }

                // Update normal fields
                existingVenue.VenueName = venue.VenueName;
                existingVenue.Location = venue.Location;
                existingVenue.Capacity = venue.Capacity;

                // Upload new image if selected
                if (venue.ImageFile != null)
                {
                    existingVenue.ImageUrl =
                        await _blobService.UploadFileAsync(venue.ImageFile);
                }

                _context.Update(existingVenue);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id);

            if (venue == null) return NotFound();

            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues
                .Include(v => v.Events)
                .ThenInclude(e => e.Bookings)
                .FirstOrDefaultAsync(v => v.VenueId == id);

            if (venue == null)
            {
                return NotFound();
            }

            // Check if venue has booked events
            bool hasBookings = venue.Events != null &&
                               venue.Events.Any(e =>
                                   e.Bookings != null &&
                                   e.Bookings.Any());

            if (hasBookings)
            {
                TempData["ErrorMessage"] =
                    "Cannot delete venue because it has active bookings.";

                return RedirectToAction(nameof(Index));
            }

            _context.Venues.Remove(venue);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
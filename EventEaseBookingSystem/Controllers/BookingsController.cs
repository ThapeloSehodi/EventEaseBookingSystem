using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEaseBookingSystem.Data;
using EventEaseBookingSystem.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EventEaseBookingSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly EventEaseContext _context;

        public BookingsController(EventEaseContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Event)
                .ThenInclude(e => e.Venue);

            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Event)
                .ThenInclude(e => e.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            if (!_context.Customers.Any() || !_context.Events.Any())
            {
                TempData["Error"] = "Please create Customers and Events first.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CustomerId = new SelectList(_context.Customers, "CustomerId", "FullName");
            ViewBag.EventId = new SelectList(_context.Events, "EventId", "EventName");
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingId,CustomerId,EventId,BookingDate")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CustomerId = new SelectList(_context.Customers, "CustomerId", "FullName", booking.CustomerId);
            ViewBag.EventId = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return NotFound();

            ViewBag.CustomerId = new SelectList(_context.Customers, "CustomerId", "FullName", booking.CustomerId);
            ViewBag.EventId = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,CustomerId,EventId,BookingDate")] Booking booking)
        {
            if (id != booking.BookingId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Bookings.Any(e => e.BookingId == booking.BookingId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CustomerId = new SelectList(_context.Customers, "CustomerId", "FullName", booking.CustomerId);
            ViewBag.EventId = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Event)
                .ThenInclude(e => e.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
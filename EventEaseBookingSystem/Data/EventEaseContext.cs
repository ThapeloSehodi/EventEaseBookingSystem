using Microsoft.EntityFrameworkCore;
using EventEaseBookingSystem.Models;

namespace EventEaseBookingSystem.Data
{
    public class EventEaseContext : DbContext
    {
        public EventEaseContext(DbContextOptions<EventEaseContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<Venue> Venues { get; set; }

        public DbSet<Booking> Bookings { get; set; }
    }
}
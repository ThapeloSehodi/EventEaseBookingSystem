using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace EventEaseBookingSystem.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(100)]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Event date is required.")]
        public DateTime EventDate { get; set; }

        public string? Description { get; set; }

        // Azure Blob Image URL
        public string? ImageUrl { get; set; }

        // Upload file (NOT stored in database)
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        // Foreign Key
        public int VenueId { get; set; }

        // Navigation Property
        public Venue? Venue { get; set; }

        // Navigation Property
        public ICollection<Booking>? Bookings { get; set; }
    }
}
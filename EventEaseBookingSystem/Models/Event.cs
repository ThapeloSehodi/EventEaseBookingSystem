using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEaseBookingSystem.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [StringLength(100)]
        public string EventName { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        public string? Description { get; set; }

        // Foreign Key
        [Required]
        public int VenueId { get; set; }

        [ForeignKey("VenueId")]
        public Venue? Venue { get; set; }

        // Navigation Property
        public ICollection<Booking>? Bookings { get; set; }
    }
}
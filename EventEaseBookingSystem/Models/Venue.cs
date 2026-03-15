using System.ComponentModel.DataAnnotations;

namespace EventEaseBookingSystem.Models
{
    public class Venue
    {
        [Key]
        public int VenueId { get; set; }

        [Required]
        [StringLength(100)]
        public string VenueName { get; set; }

        [Required]
        public string Location { get; set; }

        public int Capacity { get; set; }

        // ADD THIS LINE
        public string? ImageUrl { get; set; }

        public ICollection<Event>? Events { get; set; }
    }
}
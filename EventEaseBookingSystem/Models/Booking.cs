using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventEaseBookingSystem.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        // Foreign Key
        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }

        // Foreign Key
        [Required]
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public Event? Event { get; set; }
    }
}
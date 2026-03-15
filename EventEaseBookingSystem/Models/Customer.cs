using System.ComponentModel.DataAnnotations;

namespace EventEaseBookingSystem.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        // Navigation Property
        public ICollection<Booking>? Bookings { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace EventEaseBookingSystem.Models
{
    public class EventType
    {
        [Key]
        public int EventTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string EventTypeName { get; set; }

        public ICollection<Event>? Events { get; set; }
    }
}

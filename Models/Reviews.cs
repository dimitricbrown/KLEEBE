using System.ComponentModel.DataAnnotations;

namespace KLEEBE.Models
{
    public class Reviews
    {
        public int Id { get; set; }
        [Required]
        public string? Content { get; set; }
        [Required]
        public int Rating { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public Users? User { get; set; }
        public int RestaurantId { get; set; }
        [Required]
        public Restaurants? Restaurant { get; set; }
        public DateTime? PostedOn { get; set; }
        public DateTime? UpdatedOn {get; set; }
    }
}

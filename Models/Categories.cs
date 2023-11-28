using System.ComponentModel.DataAnnotations;

namespace KLEEBE.Models
{
    public class Categories
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? PhotoUrl { get; set; }
        public List<Restaurants>? Restaurants { get; set;}
    }
}

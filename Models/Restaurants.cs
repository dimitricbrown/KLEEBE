using System.ComponentModel.DataAnnotations;

namespace KLEEBE.Models
{
    public class Restaurants
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string? Website { get; set; }
        [Required]
        public string? PhotoUrl { get; set; }
        [Required]
        public string? VideoUrl { get; set; }
        [Required]
        public string? Address { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public DateTime? PostedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public List<Reviews>? Reviews { get; set; }
        public List<Categories>? Categories { get; set; }
    }
}

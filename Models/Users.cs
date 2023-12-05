using System.ComponentModel.DataAnnotations;

namespace KLEEBE.Models
{
    public class Users
    {
        public int Id { get; set; }
        [Required]
        public string? DisplayName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public bool IsAdmin { get; set; }
        [Required]
        public string? Uid { get; set; }
        public List<Reviews>? Reviews { get; set; }    
    }
}

using KLEEBE.Models;
using System.ComponentModel.DataAnnotations;

namespace KLEEBE.DTOs
{
    public class ReviewsDTO
    {
        public string? Content { get; set; }
        public int Rating { get; set; }
        public int UserId { get; set; }
        public int RestaurantId { get; set; }
        public DateTime? PostedOn { get; set; }
        public DateTime? UpdatedOn {get; set; }
    }
}

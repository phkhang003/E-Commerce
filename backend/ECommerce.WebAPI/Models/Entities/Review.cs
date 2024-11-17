using ECommerce.WebAPI.Models.Entities;

namespace ECommerce.WebAPI.Models.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string? Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
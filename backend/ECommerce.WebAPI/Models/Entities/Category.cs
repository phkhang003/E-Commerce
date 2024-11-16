namespace ECommerce.WebAPI.Models.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Slug { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
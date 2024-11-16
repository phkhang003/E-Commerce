namespace ECommerce.WebAPI.Models.DTOs;

/// <summary>
/// Represents a product in the system
/// </summary>
public class ProductDto
{
    /// <summary>
    /// The unique identifier for the product
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }

    /// <summary>
    /// The name of the product
    /// </summary>
    /// <example>iPhone 15</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The price of the product
    /// </summary>
    /// <example>999.99</example>
    public decimal Price { get; set; }
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
}

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
}
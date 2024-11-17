using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Models.Common;
using ECommerce.WebAPI.Common;

namespace ECommerce.WebAPI.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<PagedList<ProductDto>> GetProductsAsync(ProductFilterDto filterDto);
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
    Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
    Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
    Task DeleteProductAsync(int id);
}
using ECommerce.WebAPI.Common;
using ECommerce.WebAPI.Models.DTOs;

namespace ECommerce.WebAPI.Services.Interfaces;

public interface IProductService
{
    Task<ProductDto> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<PagedResponse<ProductDto>> GetProductsAsync(ProductFilterDto filterDto);
    Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
    Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
    Task DeleteProductAsync(int id);
    Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
}
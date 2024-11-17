using ECommerce.WebAPI.Models.DTOs;

namespace ECommerce.WebAPI.Services.Interfaces;

public interface ICategoryService
{
    Task<CategoryDto> GetCategoryByIdAsync(int id);
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto);
    Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto);
    Task DeleteCategoryAsync(int id);
}
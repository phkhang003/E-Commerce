using AutoMapper;
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Repositories.Interfaces;
using ECommerce.WebAPI.Services.Interfaces;
using ECommerce.WebAPI.Common.Exceptions;

namespace ECommerce.WebAPI.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category != null ? _mapper.Map<CategoryDto>(category) : null;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        var category = _mapper.Map<Category>(createCategoryDto);
        var result = await _categoryRepository.AddAsync(category);
        return _mapper.Map<CategoryDto>(result);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
    {
        var category = await _categoryRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Category not found");

        _mapper.Map(updateCategoryDto, category);
        await _categoryRepository.UpdateAsync(category);
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Category not found");
            
        await _categoryRepository.DeleteAsync(id);
    }
}
using AutoMapper;
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Repositories.Interfaces;
using ECommerce.WebAPI.Services.Interfaces;
using ECommerce.WebAPI.Exceptions;
using Microsoft.Extensions.Caching.Memory;

namespace ECommerce.WebAPI.Services.Implementations;

public class CategoryService : ICategoryService
{
    private const string CATEGORIES_CACHE_KEY = "categories";
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper, IMemoryCache cache)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        if (_cache.TryGetValue(CATEGORIES_CACHE_KEY, out IEnumerable<CategoryDto> cachedCategories))
            return cachedCategories;

        var categories = await _categoryRepository.GetAllAsync();
        var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10));
        _cache.Set(CATEGORIES_CACHE_KEY, categoryDtos, cacheOptions);

        return categoryDtos;
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Category with id {id} not found");
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        var category = _mapper.Map<Category>(createCategoryDto);
        var createdCategory = await _categoryRepository.AddAsync(category);
        await InvalidateCacheAsync();
        return _mapper.Map<CategoryDto>(createdCategory);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
    {
        var category = await _categoryRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Category with id {id} not found");

        _mapper.Map(updateCategoryDto, category);
        await _categoryRepository.UpdateAsync(category);
        await InvalidateCacheAsync();
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Category with id {id} not found");

        await _categoryRepository.DeleteAsync(id);
        await InvalidateCacheAsync();
    }

    public async Task InvalidateCacheAsync()
    {
        _cache.Remove(CATEGORIES_CACHE_KEY);
        await Task.CompletedTask;
    }
}
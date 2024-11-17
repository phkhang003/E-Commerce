using AutoMapper;
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Repositories.Interfaces;
using ECommerce.WebAPI.Helpers;
using ECommerce.WebAPI.Validators;
using ECommerce.WebAPI.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using ECommerce.WebAPI.Common;
using ECommerce.WebAPI.Exceptions;
using NotFoundException = ECommerce.WebAPI.Helpers.NotFoundException;

namespace ECommerce.WebAPI.Services.Implementations;

public class ProductService : IProductService
{
    private const string PRODUCTS_CACHE_KEY = "products";
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly IProductValidator _validator;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository, 
        IMapper mapper,
        IMemoryCache cache,
        IProductValidator validator)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _cache = cache;
        _validator = validator;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        if (_cache.TryGetValue(PRODUCTS_CACHE_KEY, out IEnumerable<ProductDto> cachedProducts))
            return cachedProducts;

        var products = await _productRepository.GetAllAsync();
        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10));
        _cache.Set(PRODUCTS_CACHE_KEY, productDtos, cacheOptions);

        return productDtos;
    }

    public async Task<ProductDto> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Product with id {id} not found");
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        var validationResult = await _validator.ValidateAsync(createProductDto);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(errors);
        }

        var product = _mapper.Map<Product>(createProductDto);
        await _productRepository.AddAsync(product);
        await InvalidateCacheAsync();
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Product with id {id} not found");

        if (updateProductDto.CategoryId.HasValue)
        {
            var categoryExists = await _categoryRepository.ExistsAsync(updateProductDto.CategoryId.Value);
            if (!categoryExists)
                throw new NotFoundException($"Category with id {updateProductDto.CategoryId.Value} not found");
        }

        _mapper.Map(updateProductDto, product);
        await _productRepository.UpdateAsync(product);
        await InvalidateCacheAsync();
        return _mapper.Map<ProductDto>(product);
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Product with id {id} not found");

        await _productRepository.DeleteAsync(id);
        await InvalidateCacheAsync();
    }

    public async Task<PagedResponse<ProductDto>> GetProductsAsync(ProductFilterDto filterDto)
    {
        var products = await _productRepository.GetProductsAsync(filterDto);
        var productDtos = _mapper.Map<List<ProductDto>>(products.Items);
        
        return new PagedResponse<ProductDto>(
            productDtos,
            products.TotalCount,
            products.PageNumber,
            products.PageSize
        );
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        var filterDto = new ProductFilterDto 
        { 
            SearchTerm = searchTerm,
            PageSize = int.MaxValue
        };
        
        var products = await _productRepository.GetProductsAsync(filterDto);
        return _mapper.Map<IEnumerable<ProductDto>>(products.Items);
    }

    private async Task InvalidateCacheAsync()
    {
        _cache.Remove(PRODUCTS_CACHE_KEY);
        await Task.CompletedTask;
    }
}

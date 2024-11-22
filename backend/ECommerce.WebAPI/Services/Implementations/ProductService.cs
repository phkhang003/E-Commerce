using AutoMapper;
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Repositories.Interfaces;
using ECommerce.WebAPI.Services.Interfaces;
using ECommerce.WebAPI.Common.Exceptions;
using ECommerce.WebAPI.Common;
using ECommerce.WebAPI.Models.Common;
using ECommerce.WebAPI.Validators;
using IProductValidator = ECommerce.WebAPI.Validators.IProductValidator;
using Microsoft.Extensions.Caching.Memory;

namespace ECommerce.WebAPI.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IProductValidator _validator;
        private readonly IMemoryCache _cache;
        private const string ProductListCacheKey = "ProductList";
        private readonly MemoryCacheEntryOptions _cacheOptions;

        public ProductService(
            IProductRepository productRepository, 
            ICategoryRepository categoryRepository,
            IMapper mapper,
            IProductValidator validator,
            IMemoryCache cache)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _validator = validator;
            _cache = cache;
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
        }

        public async Task<PagedList<ProductDto>> GetProductsAsync(ProductFilterDto filterDto)
        {
            var products = await _productRepository.GetProductsAsync(filterDto);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products.Items);
            return new PagedList<ProductDto>(
                productDtos.ToList(),
                products.TotalCount,
                products.PageNumber,
                products.PageSize
            );
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
        {
            var products = await _productRepository.GetProductsAsync(new ProductFilterDto { SearchTerm = searchTerm });
            return _mapper.Map<IEnumerable<ProductDto>>(products.Items);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
        {
            var validationResult = await _validator.ValidateAsync(createDto);
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException(errors);
            }

            var product = _mapper.Map<Product>(createDto);
            await _productRepository.AddAsync(product);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = await _productRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Product not found");

            if (updateProductDto.CategoryId.HasValue)
            {
                var categoryExists = await _categoryRepository.ExistsAsync(updateProductDto.CategoryId.Value);
                if (!categoryExists)
                {
                    throw new NotFoundException("Category not found");
                }
            }

            _mapper.Map(updateProductDto, product);
            await _productRepository.UpdateAsync(product);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Product not found");
                
            await _productRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            if (_cache.TryGetValue(ProductListCacheKey, out IEnumerable<ProductDto> cachedProducts))
            {
                return cachedProducts;
            }

            var products = await _productRepository.GetAllAsync();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            
            _cache.Set(ProductListCacheKey, productDtos, _cacheOptions);
            return productDtos;
        }
    }
}

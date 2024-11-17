using AutoMapper;
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Repositories.Interfaces;
using ECommerce.WebAPI.Services.Interfaces;
using ECommerce.WebAPI.Common.Exceptions;
using ECommerce.WebAPI.Common;
using ECommerce.WebAPI.Models.Common;

namespace ECommerce.WebAPI.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<PagedList<ProductDto>> GetProductsAsync(ProductFilterDto filterDto)
        {
            var products = await _productRepository.GetProductsAsync(filterDto);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
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
            return product != null ? _mapper.Map<ProductDto>(product) : null;
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
        {
            var products = await _productRepository.GetProductsAsync(new ProductFilterDto { SearchTerm = searchTerm });
            return _mapper.Map<IEnumerable<ProductDto>>(products.Items);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = _mapper.Map<Product>(createProductDto);
            var result = await _productRepository.AddAsync(product);
            return _mapper.Map<ProductDto>(result);
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = await _productRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Product not found");

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
            var products = await _productRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }
}

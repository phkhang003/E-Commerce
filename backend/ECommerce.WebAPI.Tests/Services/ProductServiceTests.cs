using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;
using ECommerce.WebAPI.Exceptions;
using FluentValidation;
using Moq;
using Xunit;
using AutoMapper;
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Repositories.Interfaces;
using ECommerce.WebAPI.Services.Implementations;
using FluentAssertions;
using ECommerce.WebAPI.Validators;
using ValidationException = ECommerce.WebAPI.Exceptions.ValidationException;
using IProductValidator = ECommerce.WebAPI.Validators.IValidator<ECommerce.WebAPI.Models.DTOs.CreateProductDto>;
using NotFoundException = ECommerce.WebAPI.Helpers.NotFoundException;
using ECommerce.WebAPI.Models.Common;

namespace ECommerce.WebAPI.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<ICategoryRepository> _mockCategoryRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IProductValidator> _mockValidator;
    private readonly Mock<IMemoryCache> _mockCache;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _mockProductRepo = new Mock<IProductRepository>();
        _mockCategoryRepo = new Mock<ICategoryRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockValidator = new Mock<IProductValidator>();
        _mockCache = new Mock<IMemoryCache>();
        
        _sut = new ProductService(
            _mockProductRepo.Object,
            _mockCategoryRepo.Object,
            _mockMapper.Object,
            _mockValidator.Object,
            _mockCache.Object
        );

        // Setup default cache behavior
        var cacheEntry = new Mock<ICacheEntry>();
        _mockCache.Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(cacheEntry.Object);
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnEmptyList_WhenNoProducts()
    {
        // Arrange
        _mockProductRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Product>());

        _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>()))
                  .Returns(new List<ProductDto>());

        // Act
        var result = await _sut.GetAllProductsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetProductById_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test Product" };
        var productDto = new ProductDto { Id = 1, Name = "Test Product" };

        _mockProductRepo.Setup(repo => repo.GetByIdAsync(1))
                       .ReturnsAsync(product);
        _mockMapper.Setup(mapper => mapper.Map<ProductDto>(product))
                  .Returns(productDto);

        // Act
        var result = await _sut.GetProductByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result?.Id.Should().Be(1);
        result?.Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnProduct_WhenValidInput()
    {
        // Arrange
        var createDto = new CreateProductDto { Name = "New Product", Price = 100 };
        var product = new Product { Id = 1, Name = "New Product", Price = 100 };
        var productDto = new ProductDto { Id = 1, Name = "New Product", Price = 100 };

        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateProductDto>()))
                     .Returns(Task.CompletedTask);

        _mockMapper.Setup(m => m.Map<Product>(createDto))
                  .Returns(product);
        
        _mockProductRepo.Setup(r => r.AddAsync(It.IsAny<Product>()))
                       .ReturnsAsync(product);
        
        _mockMapper.Setup(m => m.Map<ProductDto>(product))
                  .Returns(productDto);

        // Act
        var result = await _sut.CreateProductAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Product");
        result.Price.Should().Be(100);
    }

    [Fact]
    public async Task CreateProduct_ShouldThrowValidationException_WhenInvalidInput()
    {
        // Arrange
        var createDto = new CreateProductDto { Name = "", Price = -1 };

        _mockValidator.Setup(v => v.ValidateAsync(createDto))
            .ThrowsAsync(new ValidationException(new List<string> { "Invalid input" }));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => 
            _sut.CreateProductAsync(createDto));
    }

    [Fact]
    public async Task DeleteProduct_ShouldDeleteProduct_WhenProductExists()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test Product" };
        _mockProductRepo.Setup(repo => repo.GetByIdAsync(1))
                       .ReturnsAsync(product);
        _mockProductRepo.Setup(repo => repo.DeleteAsync(1))
                       .Returns(Task.CompletedTask);

        // Act
        await _sut.DeleteProductAsync(1);

        // Assert
        _mockProductRepo.Verify(repo => repo.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_ShouldThrowNotFoundException_WhenProductNotFound()
    {
        // Arrange
        _mockProductRepo.Setup(repo => repo.GetByIdAsync(1))
                       .ReturnsAsync((Product)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => 
            _sut.DeleteProductAsync(1));
    }

    [Fact]
    public async Task SearchProducts_ShouldReturnMatchingProducts()
    {
        // Arrange
        var products = new List<Product> 
        { 
            new Product { Id = 1, Name = "Test Product" },
            new Product { Id = 2, Name = "Another Product" }
        };
        var productDtos = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Test Product" },
            new ProductDto { Id = 2, Name = "Another Product" }
        };

        _mockProductRepo.Setup(repo => repo.SearchProductsAsync("Test"))
                       .ReturnsAsync(products);
        _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ProductDto>>(products))
                   .Returns(productDtos);

        // Act
        var result = await _sut.SearchProductsAsync("Test");

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Test Product");
    }

    [Fact]
    public async Task UpdateProduct_ShouldUpdateSuccessfully_WhenValidInput()
    {
        // Arrange
        var id = 1;
        var updateDto = new UpdateProductDto 
        { 
            Name = "Updated Product", 
            Price = 200m,
            CategoryId = 1
        };
        var existingProduct = new Product 
        { 
            Id = id, 
            Name = "Old Product", 
            Price = 100m,
            CategoryId = 1
        };

        _mockProductRepo.Setup(repo => repo.GetByIdAsync(id))
            .ReturnsAsync(existingProduct);
        _mockCategoryRepo.Setup(repo => repo.ExistsAsync(updateDto.CategoryId.Value))
            .ReturnsAsync(true);
        _mockMapper.Setup(m => m.Map(updateDto, existingProduct))
            .Returns(existingProduct);

        // Act
        await _sut.UpdateProductAsync(id, updateDto);

        // Assert
        _mockProductRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_ShouldThrowNotFoundException_WhenProductNotFound()
    {
        // Arrange
        var id = 1;
        var updateDto = new UpdateProductDto { Name = "Updated Product" };

        _mockProductRepo.Setup(repo => repo.GetByIdAsync(id))
                       .ReturnsAsync((Product)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => 
            _sut.UpdateProductAsync(id, updateDto));
    }

    [Fact]
    public async Task UpdateProduct_ShouldThrowNotFoundException_WhenCategoryNotFound()
    {
        // Arrange
        var id = 1;
        var updateDto = new UpdateProductDto { CategoryId = 1 };
        var existingProduct = new Product { Id = id, Name = "Old Product" };

        _mockProductRepo.Setup(repo => repo.GetByIdAsync(id))
                       .ReturnsAsync(existingProduct);
        _mockCategoryRepo.Setup(repo => repo.ExistsAsync(updateDto.CategoryId.Value))
                        .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => 
            _sut.UpdateProductAsync(id, updateDto));
    }

    [Fact]
    public async Task GetProducts_ShouldReturnPagedResponse_WhenUsingFilter()
    {
        // Arrange
        var filterDto = new ProductFilterDto 
        { 
            PageNumber = 1, 
            PageSize = 10,
            SearchTerm = "test"
        };
        
        var products = new List<Product> 
        { 
            new Product { Id = 1, Name = "Test Product" }
        };
        
        _mockProductRepo.Setup(repo => repo.GetPagedAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
            It.IsAny<int>(),
            It.IsAny<int>()))
            .ReturnsAsync(products);

        _mockProductRepo.Setup(repo => repo.CountAsync(It.IsAny<Expression<Func<Product, bool>>>()))
            .ReturnsAsync(1);

        // Act
        var result = await _sut.GetProductsAsync(filterDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TotalItems);
    }

    [Fact]
    public async Task GetProducts_ShouldUseCaching_WhenCalledMultipleTimes()
    {
        // Arrange
        var filterDto = new ProductFilterDto 
        { 
            PageNumber = 1, 
            PageSize = 10,
            SearchTerm = "test"
        };
        
        var products = new List<Product> 
        { 
            new Product { Id = 1, Name = "Test Product" }
        };

        var productDtos = new List<ProductDto> 
        { 
            new ProductDto { Id = 1, Name = "Test Product" }
        };

        // Create PagedResponse with 3 parameters: items, totalItems, and filterDto
        var pagedResponse = new PagedResponse<ProductDto>(productDtos, 1, filterDto);

        object cachedValue = null;
        var cacheEntry = new Mock<ICacheEntry>();
        
        _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out cachedValue))
            .Returns(false);
        
        _mockCache.Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(cacheEntry.Object);

        _mockProductRepo.Setup(repo => repo.GetPagedAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
            It.IsAny<int>(),
            It.IsAny<int>()))
            .ReturnsAsync(products);

        _mockProductRepo.Setup(repo => repo.CountAsync(It.IsAny<Expression<Func<Product, bool>>>()))
            .ReturnsAsync(1);

        _mockMapper.Setup(m => m.Map<IEnumerable<ProductDto>>(products))
            .Returns(productDtos);

        // Act
        var result1 = await _sut.GetProductsAsync(filterDto);
        
        // Update cache value
        cachedValue = pagedResponse;
        _mockCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out cachedValue))
            .Returns(true);
        
        var result2 = await _sut.GetProductsAsync(filterDto);

        // Assert
        _mockProductRepo.Verify(
            x => x.GetPagedAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                It.IsAny<int>(),
                It.IsAny<int>()), 
            Times.Once());
    }
}

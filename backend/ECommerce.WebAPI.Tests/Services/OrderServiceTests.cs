using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;
using ECommerce.WebAPI.Exceptions;
using Moq;
using Xunit;
using AutoMapper;
using ECommerce.WebAPI.Models.DTOs.Order;
using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Repositories.Interfaces;
using ECommerce.WebAPI.Services.Implementations;
using FluentAssertions;
using ECommerce.WebAPI.Models.Common;

namespace ECommerce.WebAPI.Tests.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepo;
    private readonly Mock<IProductRepository> _mockProductRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IMemoryCache> _mockCache;
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _mockOrderRepo = new Mock<IOrderRepository>();
        _mockProductRepo = new Mock<IProductRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockCache = new Mock<IMemoryCache>();
        _sut = new OrderService(
            _mockOrderRepo.Object,
            _mockProductRepo.Object,
            _mockMapper.Object,
            _mockCache.Object);
    }

    [Fact]
    public async Task CreateOrderAsync_WithValidData_ReturnsOrderDto()
    {
        // Arrange
        var userId = 1;
        var createOrderDto = new CreateOrderDto
        {
            ShippingAddress = "Test Address",
            Items = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto
                {
                    ProductId = 1,
                    Quantity = 2
                }
            }
        };

        var product = new Product { Id = 1, Name = "Test Product", Price = 100, StockQuantity = 5 };
        var expectedOrder = new Order { Id = 1, UserId = userId, TotalAmount = 200 };
        var expectedOrderDto = new OrderDto { Id = 1, TotalAmount = 200 };

        _mockProductRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);
        _mockOrderRepo.Setup(x => x.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync(expectedOrder);
        _mockMapper.Setup(x => x.Map<OrderDto>(expectedOrder))
            .Returns(expectedOrderDto);

        // Act
        var result = await _sut.CreateOrderAsync(userId, createOrderDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedOrderDto.Id);
        result.TotalAmount.Should().Be(expectedOrderDto.TotalAmount);
    }

    [Fact]
    public async Task CreateOrderAsync_WithInvalidProduct_ThrowsNotFoundException()
    {
        // Arrange
        var userId = 1;
        var createOrderDto = new CreateOrderDto
        {
            ShippingAddress = "Test Address",
            Items = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto
                {
                    ProductId = 1,
                    Quantity = 2
                }
            }
        };

        _mockProductRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await _sut.Invoking(x => x.CreateOrderAsync(userId, createOrderDto))
            .Should().ThrowAsync<NotFoundException>();
    }
}
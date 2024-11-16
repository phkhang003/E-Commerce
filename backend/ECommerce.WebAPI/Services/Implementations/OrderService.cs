using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ECommerce.WebAPI.Models.DTOs.Order;
using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Repositories.Interfaces;
using ECommerce.WebAPI.Services.Interfaces;
using ECommerce.WebAPI.Common;
using ECommerce.WebAPI.Exceptions;
using ECommerce.WebAPI.Models.Enums;

namespace ECommerce.WebAPI.Services.Implementations;

public class OrderService : IOrderService
{
    private const string USER_ORDERS_CACHE_KEY = "user_orders_{0}";
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IMapper mapper,
        IMemoryCache cache)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto createOrderDto)
    {
        var order = new Order
        {
            UserId = userId,
            ShippingAddress = createOrderDto.ShippingAddress,
            Status = OrderStatus.Pending,
            Items = new List<OrderItem>()
        };

        decimal totalAmount = 0;

        foreach (var item in createOrderDto.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId)
                ?? throw new NotFoundException($"Product with id {item.ProductId} not found");

            if (product.StockQuantity < item.Quantity)
                throw new ValidationException($"Not enough stock for product {product.Name}");

            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            };

            order.Items.Add(orderItem);
            totalAmount += product.Price * item.Quantity;
            product.StockQuantity -= item.Quantity;
        }

        order.TotalAmount = totalAmount;
        var createdOrder = await _orderRepository.AddAsync(order);
        await InvalidateOrderCacheAsync(userId);

        return _mapper.Map<OrderDto>(createdOrder);
    }

    public async Task<OrderDto> GetOrderByIdAsync(int id, int userId)
    {
        var order = await _orderRepository.GetByIdWithItemsAsync(id)
            ?? throw new NotFoundException($"Order with id {id} not found");

        if (order.UserId != userId)
            throw new UnauthorizedException("You are not authorized to view this order");

        return _mapper.Map<OrderDto>(order);
    }

    public async Task<PagedResponse<OrderDto>> GetOrdersAsync(OrderFilterDto filterDto)
    {
        var orders = await _orderRepository.GetOrdersAsync(filterDto);
        var orderDtos = _mapper.Map<List<OrderDto>>(orders.Items);
        
        return new PagedResponse<OrderDto>(
            orderDtos,
            orders.TotalCount,
            orders.PageNumber,
            orders.PageSize
        );
    }

    public async Task<OrderDto> UpdateOrderStatusAsync(int id, OrderStatus status)
    {
        var order = await _orderRepository.GetByIdWithItemsAsync(id)
            ?? throw new NotFoundException($"Order with id {id} not found");

        order.Status = status;
        await _orderRepository.UpdateAsync(order);
        await InvalidateOrderCacheAsync(order.UserId);

        return _mapper.Map<OrderDto>(order);
    }

    public async Task InvalidateOrderCacheAsync(int userId)
    {
        _cache.Remove(string.Format(USER_ORDERS_CACHE_KEY, userId));
        await Task.CompletedTask;
    }
}
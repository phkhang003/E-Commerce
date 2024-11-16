using ECommerce.WebAPI.Common;
using ECommerce.WebAPI.Models.DTOs.Order;
using ECommerce.WebAPI.Models.Enums;

namespace ECommerce.WebAPI.Services.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto createOrderDto);
    Task<OrderDto> GetOrderByIdAsync(int id, int userId);
    Task<PagedResponse<OrderDto>> GetOrdersAsync(OrderFilterDto filter);
    Task<OrderDto> UpdateOrderStatusAsync(int id, OrderStatus status);
}
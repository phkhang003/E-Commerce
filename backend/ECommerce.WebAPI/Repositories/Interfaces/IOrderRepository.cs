using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Common;
using ECommerce.WebAPI.Models.DTOs.Order;

namespace ECommerce.WebAPI.Repositories.Interfaces;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<Order?> GetByIdWithItemsAsync(int id);
    Task<IEnumerable<Order>> GetUserOrdersAsync(int userId);
    Task<PagedList<Order>> GetOrdersAsync(OrderFilterDto filter);
}
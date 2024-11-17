using Microsoft.EntityFrameworkCore;
using ECommerce.WebAPI.Data;
using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Common;
using ECommerce.WebAPI.Repositories.Interfaces;
using ECommerce.WebAPI.Models.DTOs.Order;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Order?> GetByIdWithItemsAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .Where(o => o.UserId == userId)
            .ToListAsync();
    }

    public async Task<PagedList<Order>> GetOrdersAsync(OrderFilterDto filterDto)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .AsQueryable();

        if (filterDto.Status.HasValue)
        {
            query = query.Where(o => o.Status == filterDto.Status.Value);
        }

        if (filterDto.FromDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= filterDto.FromDate.Value);
        }

        if (filterDto.ToDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= filterDto.ToDate.Value);
        }

        if (filterDto.UserId.HasValue)
        {
            query = query.Where(o => o.UserId == filterDto.UserId.Value);
        }

        return await PagedList<Order>.CreateAsync(
            query,
            filterDto.PageNumber,
            filterDto.PageSize
        );
    }
}
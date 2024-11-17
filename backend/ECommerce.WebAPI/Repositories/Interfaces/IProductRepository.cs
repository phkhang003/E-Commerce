using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Common;
using System.Linq.Expressions;

namespace ECommerce.WebAPI.Repositories.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<PagedList<Product>> GetProductsAsync(ProductFilterDto filterDto);
    Task<bool> ExistsAsync(int id);
}

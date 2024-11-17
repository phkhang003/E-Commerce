using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Common;
using System.Linq.Expressions;

namespace ECommerce.WebAPI.Repositories.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    Task<PagedList<Product>> GetProductsAsync(ProductFilterDto filterDto);
    new Task<int> CountAsync(Expression<Func<Product, bool>>? filter = null);
}

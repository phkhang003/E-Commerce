using Microsoft.EntityFrameworkCore;
using ECommerce.WebAPI.Data;
using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Common;
using ECommerce.WebAPI.Repositories.Interfaces;
using System;
using System.Linq.Expressions;

namespace ECommerce.WebAPI.Repositories.Implementations;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PagedList<Product>> GetProductsAsync(ProductFilterDto filterDto)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filterDto.SearchTerm))
        {
            query = query.Where(p => 
                p.Name.Contains(filterDto.SearchTerm) || 
                p.Description.Contains(filterDto.SearchTerm));
        }

        if (filterDto.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == filterDto.CategoryId.Value);
        }

        if (filterDto.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= filterDto.MinPrice.Value);
        }

        if (filterDto.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= filterDto.MaxPrice.Value);
        }

        query = filterDto.SortBy?.ToLower() switch
        {
            "price" => filterDto.SortOrder?.ToLower() == "desc" 
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),
            "name" => filterDto.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),
            _ => query.OrderBy(p => p.Id)
        };

        return await PagedList<Product>.CreateAsync(query, filterDto.PageNumber, filterDto.PageSize);
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        return await _context.Products
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
            .ToListAsync();
    }

    public new async Task<int> CountAsync(Expression<Func<Product, bool>>? filter = null)
    {
        var query = _context.Products.AsQueryable();
        if (filter != null)
            query = query.Where(filter);
        return await query.CountAsync();
    }
}

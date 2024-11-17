using Microsoft.EntityFrameworkCore;

namespace ECommerce.WebAPI.Common;

public class PagedList<T>
{
    public List<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }

    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Items = items;
    }

    public static async Task<PagedList<T>> CreateAsync(
        IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
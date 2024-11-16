using ECommerce.WebAPI.Models.Entities;

namespace ECommerce.WebAPI.Repositories.Interfaces;

public interface ICategoryRepository : IBaseRepository<Category>
{
    new Task<bool> ExistsAsync(int id);
}

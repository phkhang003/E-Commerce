using ECommerce.WebAPI.Models.Entities;

namespace ECommerce.WebAPI.Repositories.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
}

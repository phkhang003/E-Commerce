using ECommerce.WebAPI.Models.Entities;

namespace ECommerce.WebAPI.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdAsync(int userId);
        Task<Cart> CreateAsync(Cart cart);
        Task<Cart> UpdateAsync(Cart cart);
        Task DeleteAsync(int id);
        Task<CartItem?> GetCartItemAsync(int cartId, int productId);
    }
}
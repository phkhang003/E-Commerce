using System.Threading.Tasks;
using ECommerce.WebAPI.Models.DTOs;

namespace ECommerce.WebAPI.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);
        Task<CartDto> AddToCartAsync(int userId, AddToCartDto addToCartDto);
        Task<CartDto> UpdateCartItemAsync(int userId, int productId, UpdateCartItemDto updateCartDto);
        Task RemoveFromCartAsync(int userId, int productId);
    }
}
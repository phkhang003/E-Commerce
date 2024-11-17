using ECommerce.WebAPI.Data;
using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.WebAPI.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart> CreateAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> UpdateAsync(Cart cart)
        {
            _context.Entry(cart).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task DeleteAsync(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(i => i.CartId == cartId && i.ProductId == productId);
        }
    }
}
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Models.Entities;
using ECommerce.WebAPI.Repositories.Interfaces;
using ECommerce.WebAPI.Services.Interfaces;
using ECommerce.WebAPI.Common.Exceptions;
using AutoMapper;

namespace ECommerce.WebAPI.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public CartService(
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> AddToCartAsync(int userId, AddToCartDto addToCartDto)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var product = await _productRepository.GetByIdAsync(addToCartDto.ProductId)
                ?? throw new NotFoundException($"Product not found");

            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, product.Id);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = product.Id,
                    Quantity = addToCartDto.Quantity
                };
                cart.Items.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += addToCartDto.Quantity;
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.UpdateAsync(cart);

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> UpdateCartItemAsync(int userId, int productId, UpdateCartItemDto updateCartDto)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, productId)
                ?? throw new NotFoundException("Cart item not found");

            cartItem.Quantity = updateCartDto.Quantity;
            cart.UpdatedAt = DateTime.UtcNow;
            
            await _cartRepository.UpdateAsync(cart);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task RemoveFromCartAsync(int userId, int productId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            var cartItem = await _cartRepository.GetCartItemAsync(cart.Id, productId)
                ?? throw new NotFoundException("Cart item not found");

            cart.Items.Remove(cartItem);
            cart.UpdatedAt = DateTime.UtcNow;
            
            await _cartRepository.UpdateAsync(cart);
        }

        private async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _cartRepository.CreateAsync(cart);
            }
            return cart;
        }
    }
}
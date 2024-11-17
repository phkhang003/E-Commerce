using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Services.Interfaces;

namespace ECommerce.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<ActionResult<CartDto>> AddToCart(AddToCartDto addToCartDto)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var cart = await _cartService.AddToCartAsync(userId, addToCartDto);
            return Ok(cart);
        }

        [HttpPut("items/{productId}")]
        public async Task<ActionResult<CartDto>> UpdateCartItem(int productId, UpdateCartItemDto updateCartDto)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var cart = await _cartService.UpdateCartItemAsync(userId, productId, updateCartDto);
            return Ok(cart);
        }

        [HttpDelete("items/{productId}")]
        public async Task<ActionResult> RemoveFromCart(int productId)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            await _cartService.RemoveFromCartAsync(userId, productId);
            return NoContent();
        }
    }
}
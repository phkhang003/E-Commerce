using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ECommerce.WebAPI.Models.DTOs;
using ECommerce.WebAPI.Services.Interfaces;
using ECommerce.WebAPI.Common;
using ECommerce.WebAPI.Models.DTOs.Order;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ICurrentUserService _currentUserService;

    public OrdersController(
        IOrderService orderService,
        ICurrentUserService currentUserService)
    {
        _orderService = orderService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
    {
        var userIdStr = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userIdStr))
            return Unauthorized();
            
        var userId = int.Parse(userIdStr);
        var order = await _orderService.CreateOrderAsync(userId, createOrderDto);
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var userIdStr = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userIdStr))
            return Unauthorized();
            
        var userId = int.Parse(userIdStr);
        var order = await _orderService.GetOrderByIdAsync(id, userId);
        return Ok(order);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<OrderDto>>> GetOrders([FromQuery] OrderFilterDto filterDto)
    {
        var userIdStr = _currentUserService.GetUserId();
        if (string.IsNullOrEmpty(userIdStr))
            return Unauthorized();
            
        filterDto.UserId = int.Parse(userIdStr);
        var orders = await _orderService.GetOrdersAsync(filterDto);
        return Ok(orders);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<OrderDto>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto updateStatusDto)
    {
        var order = await _orderService.UpdateOrderStatusAsync(id, updateStatusDto.Status);
        return Ok(order);
    }
}
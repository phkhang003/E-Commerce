using ECommerce.WebAPI.Models.Enums;

namespace ECommerce.WebAPI.Models.DTOs.Order;

public class OrderDto
{
    public OrderDto()
    {
        UserEmail = string.Empty;
        Items = new List<OrderItemDto>();
    }
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserEmail { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemDto> Items { get; set; }
}

public class OrderItemDto
{
    public OrderItemDto()
    {
        ProductName = string.Empty;
    }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreateOrderDto
{
    public CreateOrderDto()
    {
        ShippingAddress = string.Empty;
        Items = new List<CreateOrderItemDto>();
    }
    public string ShippingAddress { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
}

public class CreateOrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class OrderFilterDto : IPaginationParameters
{
    public int? UserId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public OrderStatus? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
}
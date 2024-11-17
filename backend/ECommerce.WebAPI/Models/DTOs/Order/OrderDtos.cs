using System;
using System.Collections.Generic;
using ECommerce.WebAPI.Models.Common;
using ECommerce.WebAPI.Models.Enums;

namespace ECommerce.WebAPI.Models.DTOs.Order;

public class CreateOrderDto
{
    public string ShippingAddress { get; set; } = string.Empty;
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class OrderDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class OrderFilterDto : IPaginationParameters
{
    public int? UserId { get; set; }
    public OrderStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class UpdateOrderStatusDto
{
    public OrderStatus Status { get; set; }
}
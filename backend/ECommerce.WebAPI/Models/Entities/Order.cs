using ECommerce.WebAPI.Models.Enums;

namespace ECommerce.WebAPI.Models.Entities;

public class Order : BaseEntity
{
    public Order()
    {
        ShippingAddress = string.Empty;
        Items = new List<OrderItem>();
    }

    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; }
    public virtual ICollection<OrderItem> Items { get; set; }
}

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public virtual Order? Order { get; set; }
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
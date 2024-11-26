using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.WebAPI.Models.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal => Quantity * UnitPrice;
        
        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
} 
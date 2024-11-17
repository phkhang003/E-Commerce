using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.WebAPI.Models.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }
    }
} 
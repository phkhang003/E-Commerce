using System;
using System.Collections.Generic;
using ECommerce.WebAPI.Models.Entities;

namespace ECommerce.WebAPI.Models.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public List<CartItem> Items { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
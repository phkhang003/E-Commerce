namespace ECommerce.WebAPI.Models.DTOs.Payment
{
    public class PaymentRequest
    {
        public int OrderId { get; set; }
        public string? PaymentMethod { get; set; }
        public decimal Amount { get; set; }
    }
}
namespace ECommerce.WebAPI.Models.DTOs.Payment
{
    public class PaymentResponse
    {
        public string? PaymentId { get; set; }
        public string? Status { get; set; }
        public decimal Amount { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
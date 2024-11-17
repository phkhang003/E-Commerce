namespace ECommerce.WebAPI.Models.DTOs.Payment
{
    public class RefundResponse
    {
        public string? RefundId { get; set; }
        public string? PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string? Status { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
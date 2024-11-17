namespace ECommerce.WebAPI.Models.DTOs.Payment
{
    public class RefundRequest
    {
        public string? PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string? Reason { get; set; }
    }
}
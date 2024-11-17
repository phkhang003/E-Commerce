using ECommerce.WebAPI.Models.DTOs.Payment;

namespace ECommerce.WebAPI.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
        Task<PaymentStatus> GetPaymentStatusAsync(string paymentId);
        Task<RefundResponse> ProcessRefundAsync(RefundRequest request);
    }
}
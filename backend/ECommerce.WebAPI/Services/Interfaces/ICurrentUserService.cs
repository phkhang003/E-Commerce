namespace ECommerce.WebAPI.Services.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? UserName { get; }
    string? UserRole { get; }
    bool IsAuthenticated { get; }
}
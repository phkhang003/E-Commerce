namespace ECommerce.WebAPI.Services.Interfaces;

public interface ICurrentUserService
{
    string? GetUserId();
    string? GetUserEmail();
    string? GetUserRole();
}
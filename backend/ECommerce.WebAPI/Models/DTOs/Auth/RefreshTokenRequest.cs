namespace ECommerce.WebAPI.Models.DTOs.Auth;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
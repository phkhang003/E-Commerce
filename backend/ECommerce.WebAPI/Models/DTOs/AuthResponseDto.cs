using ECommerce.WebAPI.Models.DTOs.Auth;

public class AuthResponseDto
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; }

    public AuthResponseDto(string token, string refreshToken, DateTime expiresAt, UserDto user)
    {
        Token = token;
        RefreshToken = refreshToken;
        ExpiresAt = expiresAt;
        User = user;
    }
}
using ECommerce.WebAPI.Models.DTOs.Auth;

namespace ECommerce.WebAPI.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken);
    Task RevokeTokenAsync(string refreshToken);
    Task LogoutAsync();
    Task InvalidateOrderCacheAsync(int userId);
    Task<string> GeneratePasswordResetTokenAsync(string email);
    Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
}

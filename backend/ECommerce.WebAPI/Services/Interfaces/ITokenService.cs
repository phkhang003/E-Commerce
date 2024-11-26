using System.Security.Claims;
using ECommerce.WebAPI.Models.DTOs.Auth;
using ECommerce.WebAPI.Models.Entities;

namespace ECommerce.WebAPI.Services.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string GeneratePasswordResetToken();
    Task RevokeRefreshTokenAsync(string refreshToken);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}

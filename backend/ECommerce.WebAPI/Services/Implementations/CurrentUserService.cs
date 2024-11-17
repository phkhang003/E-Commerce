using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ECommerce.WebAPI.Services.Interfaces;

namespace ECommerce.WebAPI.Services.Implementations;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        }
    }

    public string? UserName
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;
        }
    }

    public string? UserRole
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
        }
    }
}
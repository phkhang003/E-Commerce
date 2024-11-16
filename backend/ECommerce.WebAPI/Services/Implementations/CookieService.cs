public class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public CookieService(
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public void SetCookie(string key, string value, CookieOptions options)
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Append(key, value, options);
    }

    public string? GetCookie(string key)
    {
        return _httpContextAccessor.HttpContext?.Request.Cookies[key];
    }

    public void DeleteCookie(string key)
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(key);
    }

    public void SetSecureCookie(string key, string value, int? expirationDays = null)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = expirationDays.HasValue 
                ? DateTime.UtcNow.AddDays(expirationDays.Value)
                : DateTime.UtcNow.AddDays(7)
        };

        SetCookie(key, value, options);
    }
}
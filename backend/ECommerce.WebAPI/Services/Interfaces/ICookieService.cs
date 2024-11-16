public interface ICookieService
{
    void SetCookie(string key, string value, CookieOptions options);
    string? GetCookie(string key);
    void DeleteCookie(string key);
    void SetSecureCookie(string key, string value, int? expirationDays = null);
}
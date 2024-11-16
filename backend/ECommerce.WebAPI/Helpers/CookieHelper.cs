public static class CookieHelper
{
    public static void SetTokenCookie(HttpContext context, string token, string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        context.Response.Cookies.Append("token", token, cookieOptions);
        context.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

    public static void RemoveTokenCookie(HttpContext context)
    {
        context.Response.Cookies.Delete("token");
        context.Response.Cookies.Delete("refreshToken");
    }
}

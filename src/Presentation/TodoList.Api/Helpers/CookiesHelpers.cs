using System.Text.Json;
using TodoList.Application.DTOs.Auth;

namespace TodoList.Api.Helpers
{
    public static class CookieHelper
    {
        public static void SetAuthCookies(HttpResponse response, string token, string refreshToken,
            SessionData sessionData)
        {
            SetCookie(response, "token", token, SameSiteMode.Strict, DateTime.UtcNow.AddMinutes(10), false);

            // Definir o cookie do refresh token
            SetCookie(response, "refreshToken", refreshToken, SameSiteMode.Strict, DateTime.UtcNow.AddDays(10), true);

            // Definir o cookie com os dados da sessão
            SetCookie(response, "sessionData", JsonSerializer.Serialize(sessionData), SameSiteMode.Strict);
        }

        // Método auxiliar para setar os cookies
        private static void SetCookie(HttpResponse response, string name, string value, SameSiteMode sameSite,
            DateTimeOffset expires = default, bool httpOnly = false)
        {
            var expiresDate = expires != default ? expires : DateTime.UtcNow.AddMinutes(10);
            response.Cookies.Append(name, value, new CookieOptions
            {
                HttpOnly = httpOnly,
                SameSite = sameSite,
                Expires = expiresDate
            });
        }

        public static void GetCookiesAuth(HttpRequest request, out string token, out string refreshToken,
            out string sessionData)
        {
            token = request.Cookies["token"];
            refreshToken = request.Cookies["refreshToken"];
            sessionData = request.Cookies["sessionData"];
        }

        public static void ClearCookiesAuth(HttpResponse response)
        {
            // Usando SetCookie para garantir que os cookies sejam removidos corretamente
            SetCookie(response, "token", "", SameSiteMode.Strict, DateTime.UtcNow.AddDays(-1), true);  // Expira no passado
            SetCookie(response, "refreshToken", "", SameSiteMode.Strict, DateTime.UtcNow.AddDays(-1), true);  // Expira no passado
            SetCookie(response, "sessionData", "", SameSiteMode.Strict, DateTime.UtcNow.AddDays(-1), true);  // Expira no passado
        }

    }
}

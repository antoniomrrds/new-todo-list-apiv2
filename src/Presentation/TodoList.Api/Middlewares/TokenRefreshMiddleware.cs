using TodoList.Api.Helpers;
using TodoList.Application.DTOs.Auth;
using TodoList.Application.ports.Repositories;
using TodoList.Application.Ports.Security;


namespace TodoList.Api.Middlewares;

public class TokenRefreshMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenRefreshMiddleware> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TokenRefreshMiddleware(RequestDelegate next,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<TokenRefreshMiddleware> logger)
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

public async Task InvokeAsync(HttpContext context)
{
    _logger.LogDebug("TokenRefreshMiddleware iniciado.");

    CookieHelper.GetCookiesAuth(context.Request, out var accessToken, out var refreshToken, out var sessionDataCookie);

    using var scope = _serviceScopeFactory.CreateScope();
    var tokenGenerator = scope.ServiceProvider.GetRequiredService<ITokenGenerator>();
    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

    // Se o access token for válido, segue direto
    if (!string.IsNullOrEmpty(accessToken))
    {
        var (isValid, userId) = await tokenGenerator.ValidateToken(accessToken);
        if (isValid)
        {

            if (string.IsNullOrEmpty(sessionDataCookie))
            {
                var userRoles = await userRepository.GetUserRolesAsync(userId);
                var roles = userRoles.Roles.Select(r => r.RoleType).ToArray();
                var sessionData = new SessionData(userRoles.Name, userRoles.Email, roles);
                CookieHelper.SetAuthCookies(context.Response, accessToken, refreshToken, sessionData);
            }

            context.Request.Headers.Authorization = $"Bearer {accessToken}"; // 🔥 Garante que o token válido está na requisição
            await _next(context);
            return;
        }
    }

    // Se o refresh token for válido, gera novos tokens e segue direto
    if (!string.IsNullOrEmpty(refreshToken))
    {
        var (isRefreshTokenValid, userId) = await tokenGenerator.ValidateToken(refreshToken);
        if (isRefreshTokenValid)
        {

            var userRoles = await userRepository.GetUserRolesAsync(userId);
            var newAccessToken = tokenGenerator.Generate(userRoles);
            var newRefreshToken = tokenGenerator.GenerateRefreshToken(userRoles);
            var roles = userRoles.Roles.Select(r => r.RoleType).ToArray();

            var sessionData = new SessionData(userRoles.Name, userRoles.Email, roles);
            CookieHelper.SetAuthCookies(context.Response, newAccessToken, newRefreshToken, sessionData);


            // 🔥 Define o novo token antes de continuar para o próximo middleware
            context.Request.Headers.Authorization = $"Bearer {newAccessToken}";

            await _next(context);
            return;
        }
    }

    await _next(context);
}
}

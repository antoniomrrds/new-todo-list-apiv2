using TodoList.Api.Helpers;
using TodoList.Application.DTOs.Auth;
using TodoList.Application.Ports.Security;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Enums;

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

        CookieHelper.GetCookiesAuth(context.Request, out var accessToken, out var refreshToken, out var sessionDataCookie);

        using var scope = _serviceScopeFactory.CreateScope();
        var tokenGenerator = scope.ServiceProvider.GetRequiredService<ITokenGenerator>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();


        // 1. Se o access token for válido, não é necessário fazer mais nada
        if (!string.IsNullOrEmpty(accessToken))
        {

            // Valida o token de acesso
            var (isValid, userId) = await tokenGenerator.ValidateToken(accessToken);

            if (isValid)
            {
                // Se o access token for válido, verifica se o sessionData existe. Se não, cria um novo.
                if (string.IsNullOrEmpty(sessionDataCookie))
                {
                    var userRoles = await userRepository.GetUserRolesAsync(userId);
                    var roles  = userRoles.Roles.Select(r => r.RoleType).ToArray();
                    var sessionData = new SessionData(userRoles.Name, userRoles.Email , roles  );
                    CookieHelper.SetAuthCookies(context.Response, accessToken, refreshToken, sessionData);
                }

                await _next(context);
                return;
            }

        }

        if (!string.IsNullOrEmpty(refreshToken))
        {

            // Valida o refresh token
            var (isRefreshTokenValid, userId) = await tokenGenerator.ValidateToken(refreshToken);

            if (isRefreshTokenValid)
            {
                // Gera novos tokens
                var userRoles = await userRepository.GetUserRolesAsync(userId);
                var newAccessToken = tokenGenerator.Generate(userRoles);
                var newRefreshToken = tokenGenerator.GenerateRefreshToken(userRoles);
                var roles  = userRoles.Roles.Select(r => r.RoleType).ToArray();

                var sessionData = new SessionData(userRoles.Name, userRoles.Email , roles  );

                // Atualiza os cookies com os novos tokens
                CookieHelper.SetAuthCookies(context.Response, newAccessToken, newRefreshToken, sessionData);

                _logger.LogInformation("Tokens renovados com sucesso.");

            }
            else
            {
                _logger.LogWarning("Refresh token inválido.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
            return;
        }

        await _next(context);
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TodoList.Application.DTOs.User;
using TodoList.Application.Ports.Security;
using TodoList.Domain.Constants;
using TodoList.Infrastructure.Helpers;

namespace TodoList.Infrastructure.Security;

public class JwtAdapter: ITokenGenerator
{

    private readonly IConfiguration _configuration;
    public JwtAdapter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Generate(UserRolesDTo data)
    {
       var jwtSettings = _configuration.GetSection("JwtSettings");
       var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["secretKey"]?? string.Empty));
       // Informações contidas no token
        List<Claim> claims =
       [
              new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
              new (JwtRegisteredClaimNames.Sub , data.Id.ToString()),
              new (JwtRegisteredClaimNames.Email, data.Email),
              new("name", data.Name),
              ..data.Roles.Select(role => new Claim(ClaimTypes.Role, ((int)role.RoleType).ToString()))
       ];
        var timeOfExpiry = jwtSettings.GetValue<int>("ExpirationTimeInMinutes");
        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(timeOfExpiry),
            signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(UserRolesDTo data)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["secretKey"]?? string.Empty));
        // Informações contidas no token
        List<Claim> claims =
        [
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, data.Id.ToString()),
            new (JwtRegisteredClaimNames.Email, data.Email),
            new("name", data.Name),
        ];
        var timeOfExpiry = jwtSettings.GetValue<int>("RefreshTokenExpirationInMinutes");
        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(timeOfExpiry),
            signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<(bool IsValid, int idUser )> ValidateToken(string token)
    {
        var responseDefault = (false, DefaultValues.IdNullValue);
        if(string.IsNullOrWhiteSpace(token)) return responseDefault;

        var tokenParameters =   Token.GetTokenValidationParameters(_configuration);
        var validTokenResult = await new JwtSecurityTokenHandler().ValidateTokenAsync(token,tokenParameters);
        if (!validTokenResult.IsValid) return responseDefault;

        var idString = validTokenResult.Claims.FirstOrDefault(c => c.Key == ClaimTypes.NameIdentifier).Value as string;
        return int.TryParse(idString, out var idUser) ? (true, idUser) : responseDefault;
    }
}

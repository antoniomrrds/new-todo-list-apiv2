using System.Security.Claims;

namespace TodoList.Domain.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        // Retorna o e-mail do usuário
        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty;
        }

        // Retorna o nome do usuário
        public static string GetName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? string.Empty;
        }

        // Retorna o ID do usuário
        public static int GetId(this ClaimsPrincipal claimsPrincipal)
        {
            var idString = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(idString, out var idUser) ? idUser : 0;
        }

        // Retorna o primeiro papel (role) do usuário
        public static string GetRole(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;
        }


        // Retorna todas as informações do usuário de uma vez
        public static UserDataClaim GetUserData(this ClaimsPrincipal claimsPrincipal)
        {
            return new UserDataClaim
            {
                IdUser = claimsPrincipal.GetId(),
                Email = claimsPrincipal.GetEmail(),
                Name = claimsPrincipal.GetName(),
                Role = claimsPrincipal.GetRole()
            };
        }
    }

    public class UserDataClaim
    {
        public int IdUser { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}

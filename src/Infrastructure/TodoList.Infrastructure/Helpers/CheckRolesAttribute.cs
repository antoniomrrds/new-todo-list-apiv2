using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TodoList.Domain.Enums;

namespace TodoList.Infrastructure.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class CheckRoles : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly Roles[] _requiredRoles;

        // Construtor para uma única role
        public CheckRoles(Roles role)
        {
            _requiredRoles = [role];
        }

        // Construtor para múltiplas roles
        public CheckRoles(params Roles[] roles)
        {
            _requiredRoles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user.Identity?.IsAuthenticated != true)
            {
                // Caso o usuário não esteja autenticado
                context.Result = new UnauthorizedResult();
                return;
            }

            var isAuthorized = UserHasRequiredRoles(user, _requiredRoles);

            if (!isAuthorized)
            {
                // Caso o usuário não tenha as permissões requeridas
                context.Result = new ForbidResult();
            }
        }

        private static bool UserHasRequiredRoles(ClaimsPrincipal user, Roles[] requiredRoles)
        {
            // Obter as roles do usuário (como int)
            var userRoles = user.FindAll(ClaimTypes.Role)
                .Select(role =>
                    int.TryParse(role.Value, out var roleValue)
                        ? (Roles?)roleValue
                        : null)
                .Where(role => role.HasValue)
                .Select(role => role ?? Domain.Enums.Roles.User) // Usar um valor padrão se não for encontrado
                .ToList();

            // Verificar se o usuário tem pelo menos uma das roles requeridas
            return requiredRoles.Intersect(userRoles).Any();
        }
    }
}

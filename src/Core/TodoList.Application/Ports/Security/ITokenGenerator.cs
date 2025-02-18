using TodoList.Application.DTOs.User;

namespace TodoList.Application.Ports.Security;

public interface ITokenGenerator
{
     string Generate(UserRolesDTo data);
     string GenerateRefreshToken(UserRolesDTo data);
     Task<(bool IsValid, int idUser)> ValidateToken(string token);

}

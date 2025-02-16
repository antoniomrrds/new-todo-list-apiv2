using TodoList.Application.DTOs.User;

namespace TodoList.Application.ports.Repositories;

public interface IUserRepository
{
    Task<bool> DoesEmailExist(string email);
    Task<IEnumerable<UserRolesDTo>> GetUserRolesAsync(int idUser);
}

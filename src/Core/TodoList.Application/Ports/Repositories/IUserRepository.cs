using TodoList.Application.DTOs.User;
using TodoList.Domain.Entities;

namespace TodoList.Application.ports.Repositories;

public interface IUserRepository
{
    Task<bool> DoesEmailExist(string email);
    Task<UserRolesDTo> GetUserRolesAsync(int idUser);
    Task<User> GetUserByEmailAsync(string email);
    Task<User> GetUserByIdAsync(int id);
}

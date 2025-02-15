using TodoList.Application.DTOs.Auth;
using TodoList.Domain.Entities;

namespace TodoList.Application.ports.Repositories;

public interface IAuthRepository
{
    public Task<int> RegisterAsync(User user);
}

using TodoList.Domain.Entities;

namespace TodoList.Application.ports.Repositories;

public interface IAuthRepository
{
    public Task<int> SignUpUserAsync(User user);
    // public Task<string>GenerateRefreshTokenAsync(int idUser);
}

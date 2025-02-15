namespace TodoList.Application.ports.Repositories;

public interface IUserRepository
{
    Task<bool> DoesEmailExist(string email);
}

namespace TodoList.Application.ports.Repositories;

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(int id);
}
using TodoList.Domain.Entities;

namespace TodoList.Application.ports.Repositories;

public interface ITodoRepository : IRepository<Todo>
{
    Task<int> CreateAsync(Todo todo);
    Task<IEnumerable<Todo>> GetAllAsync();
    Task<int> UpdateAsync(Todo todo);
    Task<int> DeleteAsync(int id);
}
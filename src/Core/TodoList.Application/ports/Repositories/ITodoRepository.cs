
using TodoList.Domain.Entities;

namespace TodoList.Application.ports.Repositories;

public interface ITodoRepository
{
    Task<int> CreateAsync(Todo todo);
    Task<IEnumerable<Todo>> GetAllAsync();
    Task<Todo?> GetByIdAsync(int id);
    Task<int> UpdateAsync(Todo todo);
    Task<int> DeleteAsync(int id);
}
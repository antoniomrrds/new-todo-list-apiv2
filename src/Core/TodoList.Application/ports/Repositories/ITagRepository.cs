using TodoList.Domain.Entities;

namespace TodoList.Application.ports.Repositories;

public interface ITagRepository
{
    Task<int> CreateAsync(Tag tag);
    Task<IEnumerable<Tag>> GetAllAsync();
    Task<Tag?> GetByIdAsync(int id);
    Task<int> UpdateAsync(Tag todo);
    Task<int> DeleteAsync(int id);
}
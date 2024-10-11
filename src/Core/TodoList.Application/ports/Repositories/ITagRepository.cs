using TodoList.Application.DTOs.Tag;
using TodoList.Domain.Entities;

namespace TodoList.Application.ports.Repositories
{
    public interface ITagRepository
    {
        Task<int> CreateAsync(Tag tag);
        Task<IEnumerable<Tag>> GetAllTagsWithDetailsAsync();
        Task<Tag?> GetByIdAsync(int id); 
        Task<int> UpdateAsync(Tag tag); 
        Task<int> DeleteTagByIdAsync(int id); 
    }
}
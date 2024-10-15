using TodoList.Domain.Entities;

namespace TodoList.Application.ports.Repositories
{
    public interface ITagRepository: IRepository<Tag>
    {
        Task<int> CreateAsync(Tag tag);
        Task<IEnumerable<Tag>> GetAllTagsWithDetailsAsync();
        Task<int> UpdateAsync(Tag tag); 
        Task<int> DeleteTagByIdAsync(int id); 
    }
}
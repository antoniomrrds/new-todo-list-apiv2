using TodoList.Application.DTOs.Tag;

namespace TodoList.Application.ports.Repositories
{
    public interface ITagRepository
    {
        Task<int> CreateAsync(TagCreateDTo tagCreateDto);
        Task<IEnumerable<TagDTo>> GetAllAsync();
        Task<TagDTo?> GetByIdAsync(int id); 
        Task<int> UpdateAsync(TagUpdateDTo tagUpdateDto); 
        Task<int> DeleteAsync(int id); 
    }
}
using TodoList.Application.DTOs.Tag;
using TodoList.Domain.Entities;

namespace TodoList.Application.ports.Repositories
{
  public interface ITagRepository : IEntityExistenceCheckerRepository
  {
    Task<int> CreateAsync(Tag tag);
    Task<IEnumerable<Tag>> GetAllAsync();
    Task<int> UpdateAsync(Tag tag);
    Task<int> DeleteTagByIdAsync(int id);
    Task<Tag> GetByIdAsync(int id);
    Task<(IEnumerable<Tag> Items ,int TotalItems)> FindByFilter(TagFilterDTo filter, int start = 0, int offset = 0);
  }
}

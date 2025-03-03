using TodoList.Application.DTOs.Category;
using TodoList.Domain.Entities;

namespace TodoList.Application.ports.Repositories;

public interface ICategoryRepository: IEntityExistenceCheckerRepository
{
    Task<int> CreateAsync(Category category);
    Task<IEnumerable<Category>> GetAllCategoriesWithDetailsAsync();
    Task<int> UpdateAsync(Category category);
    Task<int> DeleteCategoryByIdAsync(int id);
    Task<Category> GetByIdAsync(int id);
    Task<(IEnumerable<Category> Items ,int TotalItems)> FindByFilter(CategoryFilterDTo filter, int start = 0, int offset = 0);

}

using TodoList.Domain.Entities;

namespace TodoList.Application.ports.Repositories;

public interface ICategoryRepository: IEntityExistenceChecker
{
    Task<int> CreateAsync(Category category);
    Task<IEnumerable<Category>> GetAllCategoriesWithDetailsAsync();
    Task<int> UpdateAsync(Category category);
    Task<int> DeleteCategoryByIdAsync(int id);
    Task<Category?> GetByIdAsync(int id);
}
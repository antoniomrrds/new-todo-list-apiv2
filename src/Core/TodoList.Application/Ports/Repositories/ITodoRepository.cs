
using TodoList.Application.DTOs.Shared;
using TodoList.Application.DTOs.Todo;
using TodoList.Domain.Entities;

namespace TodoList.Application.ports.Repositories;

public interface ITodoRepository 
{
    Task<int> CreateAsync(CreateTodoDTo createTodoDTo);
    Task<IEnumerable<Todo>> GetAllAsync();
    Task<int> UpdateAsync(UpdateTodoDTo updateTodoDTo);
    Task<int> DeleteAsync(int id);
    Task<Todo> GetByIdAsync(int id);
    Task<TodoWithTagAndCategoryIdsDto> GetTodoWithTagAndCategoryIdsAsync(int id);
    Task<TodoWithTagsAndCategoriesDTo> GetTodoWithTagsAndCategoriesAsync(int id);
    Task<(IEnumerable<Todo> Items, int TotalItems)> FindByFilter(ToDoFilterDTo filter, int start = 0, int offset = 0);
    
}
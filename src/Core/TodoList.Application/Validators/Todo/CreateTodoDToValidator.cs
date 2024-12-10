using TodoList.Application.DTOs.Todo;
using TodoList.Application.ports.Repositories;
namespace TodoList.Application.Validators.Todo;
public class CreateTodoDToValidator(ITagRepository tagRepository, ICategoryRepository categoryRepository): 
    TodoDToBaseWithTagAndCategoryIds<CreateTodoDTo>(tagRepository, categoryRepository);
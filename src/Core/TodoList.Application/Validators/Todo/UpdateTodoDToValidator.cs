using TodoList.Application.DTOs.Todo;
using TodoList.Application.ports.Repositories;
namespace TodoList.Application.Validators.Todo;

public class UpdateDToValidator(ITagRepository tagRepository , ICategoryRepository categoryRepository) :
    TodoDToBaseWithTagAndCategoryIds<UpdateTodoDTo>(tagRepository,categoryRepository);
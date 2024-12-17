using TodoList.Application.Ports.Validators;

namespace TodoList.Application.DTOs.Category;

public class CreateCategoryDTo : CommonPropertiesTagAndCategory
{ }

public class UpdateCategoryDTo : CommonPropertiesTagAndCategory
{
    public DateTime UpdatedAt { get; set; }
}

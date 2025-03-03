using TodoList.Application.Ports.Validators;
using TodoList.Domain.Enums;

namespace TodoList.Application.DTOs.Category;

public class CreateCategoryDTo : CommonPropertiesTagAndCategory
{ }

public class UpdateCategoryDTo : CommonPropertiesTagAndCategory
{
    public DateTime UpdatedAt { get; set; }
}
public class CategoryFilterDTo :CommonPropertiesTagAndCategory
{
    public int Page { get; set; }
    public int IdUser { get; set; }
    public int PageSize { get; set; }
}

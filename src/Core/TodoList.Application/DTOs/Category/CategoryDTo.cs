using TodoList.Application.Constants;

namespace TodoList.Application.DTOs.Category;

public record CreateCategoryDTo(
    string Name , 
    string Description,
    int Active = DefaultValues.Active
    );

public record CategoryDTo(
        int Id,
        string Name,
        string Description,
        int Active,
        DateTime CreatedAt,
        DateTime UpdatedAt);

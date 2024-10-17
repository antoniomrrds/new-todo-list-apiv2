namespace TodoList.Application.DTOs.Category;

public record CreateCategoryDTo(string Name);

public record CategoryDTo(
        int Id,
        string Name,
        DateTime CreatedAt,
        DateTime UpdatedAt);

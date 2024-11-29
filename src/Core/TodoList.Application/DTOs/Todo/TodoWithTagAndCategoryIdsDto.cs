namespace TodoList.Application.DTOs.Todo;

public record TodoWithTagAndCategoryIdsDto(
    int Id,
    string Title,
    string Description,
    bool IsCompleted,
    int Active,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? ExpirationDate,
    string? ExpirationDateFormatted,
    string? CreatedAtFormatted,
    string? UpdatedAtFormatted,
    List<int>? IdTags,
    List<int>? IdCategories);
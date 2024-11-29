using TodoList.Application.Constants;

namespace TodoList.Application.DTOs.Todo;

public record UpdateTodoDTo(
    int Id,
    string Title,
    string Description,
    List<int>? IdTags ,
    List<int>? IdCategories ,
    int Active = DefaultValues.Active,
    bool IsCompleted = false,
    DateTime? ExpirationDate = null
)
{
    public DateTime UpdatedAt { get; init; }
}

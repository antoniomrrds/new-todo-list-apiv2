using TodoList.Application.Constants;
namespace TodoList.Application.DTOs.Todo;

public record CreateTodoDTo(
    List<int>? IdTags,
    List<int>? IdCategories,
    string Title,
    string Description,
    bool IsCompleted = false,
    int Active = DefaultValues.Active,
    DateTime? ExpirationDate = null
)
{
    public DateTime CreatedAt {  set; get; }
    public DateTime UpdatedAt { get; set; }
}

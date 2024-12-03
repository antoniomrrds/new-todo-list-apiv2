using TodoList.Application.Constants;
using TodoList.Domain.Entities;

namespace TodoList.Application.DTOs.Todo;

public class UpdateTodoDTo:ITodo, ITagAndCategoryIdsDto {
    public int Id { get; init; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int Active { get; set; } = DefaultValues.Active;
    public DateTime? ExpirationDate { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<int>? IdTags { get; set; }
    public List<int>? IdCategories { get; set; }
}

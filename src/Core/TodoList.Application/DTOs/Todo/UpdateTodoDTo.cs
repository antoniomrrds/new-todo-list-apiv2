
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Application.DTOs.Todo;

public class UpdateTodoDTo:TagAndCategoryIdsDto , ITodo
{
    public int Id { get; init; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public ActivationState Active { get; set; }
    public DateTime UpdatedAt { get; set; }
}

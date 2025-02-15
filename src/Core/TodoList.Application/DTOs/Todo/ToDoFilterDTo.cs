using TodoList.Domain.Constants;
using TodoList.Domain.Enums;

namespace TodoList.Application.DTOs.Todo;

public class ToDoFilterDTo
{
    public int Page { get; set; }
    public string Name { get; set; } = string.Empty;
    public TodoStatus Status { get; set; } = TodoStatus.InProgress;
    public int PageSize { get; set; }
    public ActivationState Active { get; set; }
    public bool IsActive => Active == ActivationState.Active;
}

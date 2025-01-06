using TodoList.Domain.Enums;

namespace TodoList.Application.DTOs.Todo;

public class ToDoFilterDTo
{
    public int Page { get; set; }
    public string Title { get; set; } = string.Empty;
    public ActivationState Active { get; set; }
    public int PageSize { get; set; }
    public bool IsActive => Active == ActivationState.Active;
}
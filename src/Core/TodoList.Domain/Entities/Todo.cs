using TodoList.Domain.Enums;

namespace TodoList.Domain.Entities;

public interface ITodo
{
    string Title { get; set; }
    string Description { get; set; }
    bool IsCompleted { get; set; } 
    DateTime? ExpirationDate { get; set; }
    ActivationState State { get; set; }
}

public sealed class Todo : BaseEntity , ITodo
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public ActivationState State { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? ExpirationDateFormatted { get;  init; }
    public string? CreatedAtFormatted { get;  init; }
    public string? UpdatedAtFormatted { get;  init; }
}
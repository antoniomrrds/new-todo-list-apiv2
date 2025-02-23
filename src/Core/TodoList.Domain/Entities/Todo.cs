using TodoList.Domain.Constants;
using TodoList.Domain.Enums;
using TodoList.Domain.extensions;
using TodoList.Domain.Extensions;

namespace TodoList.Domain.Entities;

public interface ITodo
{
    string Name { get; set; }
    string Description { get; set; }
    CompletionStatus IsCompleted { get; set; }
    DateTime? ExpirationDate { get; set; }
    ActivationState Active { get; set; }
}

public sealed class Todo : BaseEntity , ITodo
{
    public int IdUser { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
    public CompletionStatus IsCompleted { get; set; }
    public ActivationState Active { get; set; }
    public string StatusDescription => Status.GetDescription();
    public TodoStatus Status { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? ExpirationDateFormatted { get;  init; }
    public string? CreatedAtFormatted { get;  init; }
   public bool IsExpirationDateValid  => ExpirationDate.HasValue && ExpirationDate.Value < DateTime.UtcNow;
    public string? UpdatedAtFormatted { get;  init; }
}

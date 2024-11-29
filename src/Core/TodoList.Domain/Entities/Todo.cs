namespace TodoList.Domain.Entities;

public sealed class Todo : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? ExpirationDate { get; set; }
    public int Active { get; set; } = Constants.DefaultValues.Active;
    public bool IsCompleted { get; set; }
    public string? ExpirationDateFormatted { get;  init; }
    public string? CreatedAtFormatted { get;  init; }
    public string? UpdatedAtFormatted { get;  init; }
}



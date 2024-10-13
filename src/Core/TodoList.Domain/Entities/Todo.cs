namespace TodoList.Domain.Entities;

public sealed class Todo : BaseEntity
{
    public int? IdTag { get; set; }
    public int? IdCategory { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Status { get; set; } = Constants.DefaultValues.Active;
    public bool IsCompleted { get; set; } =  false;
}
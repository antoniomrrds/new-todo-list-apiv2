using TodoList.Domain.Constants;

namespace TodoList.Domain.Entities;

public sealed class Category: BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Active { get; init; } = DefaultValues.Active;
}
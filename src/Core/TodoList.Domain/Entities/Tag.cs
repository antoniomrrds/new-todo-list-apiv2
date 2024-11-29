using TodoList.Domain.Constants;

namespace TodoList.Domain.Entities;

public sealed class Tag: BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#FFFFFF";
    public string Description { get; set; } = string.Empty;
    public int Active { get; set; } = DefaultValues.Active;

}
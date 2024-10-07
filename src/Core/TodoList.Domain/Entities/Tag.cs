
using TodoList.Domain.Constants;

namespace TodoList.Domain.Entities;
public sealed class Tag: BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int Status { get; set; } = DefaultValues.Active;
}
using TodoList.Domain.Enums;

namespace TodoList.Domain.Entities;

public sealed class Category: BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ActivationState Active{ get; init; }
    public void SetDateOfCreation() {
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }
}
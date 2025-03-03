using TodoList.Domain.Enums;

namespace TodoList.Domain.Entities;

public sealed class Category: BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ActivationState Active{ get; init; }
    public string CreatedAtFormatted => CreatedAt.ToString("dd/MM/yyyy HH:mm:ss");
    public string UpdatedAtFormatted => UpdatedAt.ToString("dd/MM/yyyy HH:mm:ss");
    public void SetDateOfCreation() {
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }
}

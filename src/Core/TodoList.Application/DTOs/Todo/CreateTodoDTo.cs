using TodoList.Domain.Enums;
namespace TodoList.Application.DTOs.Todo;

public class CreateTodoDTo :TagAndCategoryIdsDto
{
    public string Title { get; set; }  = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public ActivationState Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public void SetDateOfCreation() {
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }
}


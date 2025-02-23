using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
namespace TodoList.Application.DTOs.Todo;

public class CreateTodoDTo :TagAndCategoryIdsDto , ITodo
{
    public int IdUser { get; set; }
    public string Name { get; set; }  = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CompletionStatus IsCompleted { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public ActivationState Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public void SetDateOfCreation() {
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }
}


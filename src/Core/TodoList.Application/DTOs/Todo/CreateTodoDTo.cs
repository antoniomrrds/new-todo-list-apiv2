using TodoList.Application.Constants;
using TodoList.Domain.Entities;

namespace TodoList.Application.DTOs.Todo;

public class CreateTodoDTo : ITodo
{
    public List<int>? IdTags { get; init; }
    public List<int>? IdCategories { get; init; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int Active { get; set; } = DefaultValues.Active;
    public DateTime? ExpirationDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public void SetDateOfCreation() {
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }  
}

using TodoList.Domain.Enums;

namespace TodoList.Application.DTOs.Todo;

public class TodoWithTagAndCategoryIdsDto: TagAndCategoryIdsDto 
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public ActivationState Active { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? ExpirationDateFormatted { get; set;  }
    public string? CreatedAtFormatted { get; set;  }
    public string? UpdatedAtFormatted { get; set;  }
    
}

public abstract class TagAndCategoryIdsDto
{
    public List<int>? IdTags { get; set; } = [];
    public List<int>? IdCategories { get; set; } = [];
}

public class TagAndCategoryFieldsDTo : TagAndCategoryIdsDto
{ }


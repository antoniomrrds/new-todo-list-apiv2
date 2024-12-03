using Entities = TodoList.Domain.Entities;
namespace TodoList.Application.DTOs.Todo;

public record TodoWithTagsAndCategoriesDTo
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsCompleted { get; init; }
    public int Active { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime? ExpirationDate { get; init; }
    public string? ExpirationDateFormatted { get; init; }
    public string? CreatedAtFormatted { get; init; }
    public string? UpdatedAtFormatted { get; init; }
    public List<Entities.Tag> Tags { get; init; } = [];
    public List<Entities.Category> Categories { get; init; } = [];
}

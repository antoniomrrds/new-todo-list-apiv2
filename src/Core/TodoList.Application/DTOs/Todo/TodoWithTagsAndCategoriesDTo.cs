using TodoList.Domain.Constants;
using Entities = TodoList.Domain.Entities;
namespace TodoList.Application.DTOs.Todo;

public record TodoWithTagsAndCategoriesDTo(
    int Id ,
    string Title,
    string Description,
    bool IsCompleted,
    int Active,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? ExpirationDate,
    string? ExpirationDateFormatted,
    string? CreatedAtFormatted,
    string? UpdatedAtFormatted,
    List<Entities.Tag> Tags,
    List<Entities.Category> Categories
    );

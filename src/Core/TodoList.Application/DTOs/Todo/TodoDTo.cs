
using TodoList.Domain.Enums;

namespace TodoList.Application.DTOs.Todo;

public record TodoDTo(
    int Id,
    string Title,
    string Description,
    bool IsCompleted,
    ActivationState Active,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? ExpirationDate,
    string? ExpirationDateFormatted,
    string? CreatedAtFormatted,
    string? UpdatedAtFormatted    
);



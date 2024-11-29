using TodoList.Application.Constants;

namespace TodoList.Application.DTOs.Tag;

public record CreateTagDTo(
    string Name,
    string Description,
    string Color,
    int Active = DefaultValues.Active);


public record TagDTo(
    int Id,
    string Name,
    string Description,
    string Color,
    int Active,
    DateTime CreatedAt,
    DateTime UpdatedAt
    )
{
    public string CreatedAtFormatted => CreatedAt.ToString("dd/MM/yyyy HH:mm");
    public string UpdatedAtFormatted => UpdatedAt.ToString("dd/MM/yyyy HH:mm");
    
};
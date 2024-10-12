using TodoList.Application.Constants;

namespace TodoList.Application.DTOs.Tag;

public record TagCreateDTo(
    string Name,
    string Slug,
    string Description,
    string Color,
    int Status = DefaultValues.Active);


public record TagDTo(
    int Id,
    string Name,
    string Slug,
    string Description,
    string Color,
    int Status,
    DateTime CreatedAt,
    DateTime UpdatedAt
    )
{
    public string CreatedAtFormatted => CreatedAt.ToString("dd/MM/yyyy HH:mm");
    public string UpdatedAtFormatted => UpdatedAt.ToString("dd/MM/yyyy HH:mm");
    
};
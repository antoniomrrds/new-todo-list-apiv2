using TodoList.Application.Constants;

namespace TodoList.Application.DTOs.Tag;

public record TagCreateDTo(
    string Name,
    string Slug,
    string Description,
    string Color,
    int Status = DefaultValues.Active);

public record TagUpdateDTo(
    string? Name,              
    string? Slug,
    string? Description,
    string? Color,
    int? Status);

public record TagDTo(
    int Id,
    string Name,
    string Slug,
    string Description,
    string Color,
    int Status);
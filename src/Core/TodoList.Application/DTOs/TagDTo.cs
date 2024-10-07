using TodoList.Application.Constants;

namespace TodoList.Application.DTOs;

public record TagDTo(
    string Name,
    string Slug,
    string Description,
    string Color,
    int Status = DefaultValues.Active);
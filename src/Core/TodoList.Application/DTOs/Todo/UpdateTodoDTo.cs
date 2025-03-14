﻿
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Application.DTOs.Todo;

public class UpdateTodoDTo:TagAndCategoryIdsDto , ITodo
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CompletionStatus IsCompleted { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public ActivationState Active { get; set; }
    public DateTime UpdatedAt { get; set; }
}

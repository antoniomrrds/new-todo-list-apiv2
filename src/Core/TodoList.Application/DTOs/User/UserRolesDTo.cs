using TodoList.Domain.Enums;

namespace TodoList.Application.DTOs.User;

public record UserRolesDTo
{
    public int Id  { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public ActivationState Active { get; init; }
    public int IdRole { get; init; }
    public  string RoleName { get; init; } = string.Empty;
    public int RoleType { get; init; }
}

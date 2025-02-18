using TodoList.Domain.Enums;

namespace TodoList.Application.DTOs.User;

public record UserRolesDTo
{
    public int Id  { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public ActivationState Active { get; init; }

    public IEnumerable<RolesDTO> Roles { get; private set; } = new List<RolesDTO>();

    public void SetRoles(IEnumerable<RolesDTO> roles)
    {
        Roles = roles;
    }
}

public record RolesDTO
{
    public int Id { get; init; }
    public  string Name { get; init; } = string.Empty;
    public Roles RoleType { get; init; }
}

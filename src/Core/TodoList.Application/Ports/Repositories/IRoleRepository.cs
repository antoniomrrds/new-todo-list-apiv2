using System.Data;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Application.ports.Repositories;

public interface IRoleRepository
{
    public Task<bool> RoleExists(Roles role);
    public Task<int> CreateAsync(Role role, IDbConnection connection, IDbTransaction transaction);
    public Task AssociateRoleToUserAsync(int userId, int roleId, IDbConnection connection, IDbTransaction transaction);
    public Task<int?> GetRoleIdByRoleTypeAsync(Roles roleType, IDbConnection connection, IDbTransaction transaction);
}

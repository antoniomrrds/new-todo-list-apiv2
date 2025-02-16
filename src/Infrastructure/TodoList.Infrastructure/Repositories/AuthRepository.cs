using System.Text;
using Dapper;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Infrastructure.Repositories;

public class AuthRepository:IAuthRepository
{
    private readonly IDatabaseExecutor _dataBaseExecutor;
    private readonly IRoleRepository _roleRepository;

    public AuthRepository(IDatabaseExecutor dataBaseExecutor , IRoleRepository roleRepository)
    {
        _dataBaseExecutor = dataBaseExecutor;
        _roleRepository =  roleRepository;
    }

    public async Task<int> SignUpUserAsync(User user)
    {
        user.SetCreateAndUpdateDate();

        var sql = new StringBuilder();
        sql.AppendLine("INSERT INTO tbl_user(    ");
        sql.AppendLine("       NAME,             ");
        sql.AppendLine("       EMAIL,            ");
        sql.AppendLine("       PASSWORD,         ");
        sql.AppendLine("       ACTIVE,           ");
        sql.AppendLine("       CREATED_AT,       ");
        sql.AppendLine("       UPDATED_AT        ");
        sql.AppendLine(") VALUES (               ");
        sql.AppendLine("       @Name,            ");
        sql.AppendLine("       @Email,           ");
        sql.AppendLine("       @Password,        ");
        sql.AppendLine("       @Active,          ");
        sql.AppendLine("       @CreatedAt,       ");
        sql.AppendLine("       @UpdatedAt        ");
        sql.AppendLine(");                       ");
        sql.AppendLine("SELECT LAST_INSERT_ID();");

        return await _dataBaseExecutor.ExecuteWithTransactionAsync(async (connection, transaction) =>
        {
            var userId = await connection.QuerySingleAsync<int>(sql.ToString(), user, transaction);
            var roleId = await _roleRepository.GetRoleIdByRoleTypeAsync(Roles.User, connection, transaction);
            if (roleId is null or 0)
            {
                // If role is not found, throw an exception to trigger rollback
                throw new InvalidOperationException("Role not found for the specified role type.");
            }

            await _roleRepository.AssociateRoleToUserAsync(userId, roleId.Value, connection, transaction);
            return userId;
        });
    }
}

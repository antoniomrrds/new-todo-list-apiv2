using System.Data;
using System.Text;
using Dapper;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Infrastructure.Repositories;

public class RoleRepository:IRoleRepository
{
    private readonly IDatabaseExecutor _dataBaseExecutor;

    public RoleRepository(IDatabaseExecutor dataBaseExecutor)
    {
        _dataBaseExecutor = dataBaseExecutor;
    }

   public async Task<bool> RoleExists(Roles role)
   {
       var sql = new StringBuilder();
       sql.AppendLine("SELECT EXISTS (                ");
       sql.AppendLine("    SELECT 1                   ");
       sql.AppendLine("    FROM tbl_role              ");
       sql.AppendLine("    WHERE role_type = @RoleType");
       sql.AppendLine(") AS RoleExists;               ");

       return await _dataBaseExecutor.ExecuteAsync<bool>(async con =>
           await con.QueryFirstAsync<bool>(sql.ToString(), new { RoleType = role }));
   }

   public async Task<int> CreateAsync(Role role, IDbConnection connection, IDbTransaction transaction)
   {
       var sql = new StringBuilder();
       sql.AppendLine("INSERT IGNORE INTO tbl_role(   ");
       sql.AppendLine("       NAME,                  ");
       sql.AppendLine("       DESCRIPTION,           ");
       sql.AppendLine("       ROLE_TYPE,             ");
       sql.AppendLine("       CREATED_AT,            ");
       sql.AppendLine("       UPDATED_AT             ");
       sql.AppendLine(") VALUES (                    ");
       sql.AppendLine("       @Name,                 ");
       sql.AppendLine("       @Description,          ");
       sql.AppendLine("       @RoleType,             ");
       sql.AppendLine("       @CreatedAt,            ");
       sql.AppendLine("       @UpdatedAt             ");
       sql.AppendLine(");                            ");
       sql.AppendLine("SELECT LAST_INSERT_ID();      ");
       var roleId = await connection.QuerySingleAsync<int>(sql.ToString(), role, transaction);
       return roleId;
   }

   public async Task AssociateRoleToUserAsync(int userId, int roleId, IDbConnection connection, IDbTransaction transaction)
   {
       var sql = new StringBuilder();
       sql.AppendLine("INSERT INTO tbl_user_role(    ");
       sql.AppendLine("       ID_USER,               ");
       sql.AppendLine("       ID_ROLE                ");
       sql.AppendLine(") VALUES (                    ");
       sql.AppendLine("       @UserId,               ");
       sql.AppendLine("       @RoleId                ");
       sql.AppendLine(");                            ");
       await connection.ExecuteAsync(sql.ToString(), new { UserId = userId, RoleId = roleId }, transaction);
   }

   public async Task<int?> GetRoleIdByRoleTypeAsync(Roles roleType, IDbConnection connection,
       IDbTransaction transaction)
   {
       var sql = new StringBuilder();
       sql.AppendLine("SELECT ID                   ");
       sql.AppendLine("FROM tbl_role               ");
       sql.AppendLine("WHERE role_type = @RoleType ");
       sql.AppendLine("LIMIT 1;                    ");
       return await connection.QueryFirstOrDefaultAsync<int>(sql.ToString(), new { RoleType = roleType }, transaction);
   }
}

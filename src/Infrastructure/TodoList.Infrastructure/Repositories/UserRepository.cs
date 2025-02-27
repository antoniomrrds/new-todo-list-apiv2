using System.Text;
using Dapper;
using TodoList.Application.DTOs.User;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;

namespace TodoList.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDatabaseExecutor _dataBaseExecutor;

    public UserRepository(IDatabaseExecutor dataBaseExecutor)
    {
        _dataBaseExecutor = dataBaseExecutor;
    }

    public async Task<bool> DoesEmailExist(string email)
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT EXISTS (         ");
        sql.AppendLine("    SELECT 1            ");
        sql.AppendLine("    FROM tbl_user       ");
        sql.AppendLine("    WHERE email = @Email");
        sql.AppendLine(") AS EmailExists;       ");

        return await _dataBaseExecutor.ExecuteAsync<bool>(async con =>
            await con.QueryFirstAsync<bool>(sql.ToString(), new { Email = email }));
    }

    public async Task<UserRolesDTo> GetUserRolesAsync(int idUser)
    {
        var sql = BuildUserQuery();
        sql.AppendLine("FROM tbl_user TU ");
        sql.AppendLine("WHERE TU.ID = @IdUser;");
        sql.AppendLine();
        sql.Append(GetRoles());

        return await _dataBaseExecutor.ExecuteAsync<UserRolesDTo>(async con =>
        {
            await using var multi = await con.QueryMultipleAsync(sql.ToString(), new { IdUser = idUser });
            var user = await multi.ReadSingleOrDefaultAsync<UserRolesDTo>();
            var roles = (await multi.ReadAsync<RolesDTO>()).ToList();
            if (user is null) return new UserRolesDTo();
            user.SetRoles(roles);
            return user;
        });
    }

    public Task<User> GetUserByEmailAsync(string email)
    {
        var sql = GetBaseQuery();
        sql.AppendLine("WHERE TU.EMAIL = @Email;");
        return _dataBaseExecutor.ExecuteAsync<User>(async con =>
            await con.QueryFirstOrDefaultAsync<User>(sql.ToString(), new { Email = email }) ?? new User())
     ;
    }

    public Task<User> GetUserByIdAsync(int id)
    {
        var sql = GetBaseQuery();
        sql.AppendLine("WHERE TU.ID = @Id;");
        return _dataBaseExecutor.ExecuteAsync<User>(async con =>
            await con.QueryFirstOrDefaultAsync<User>(sql.ToString(), new { Id = id }) ?? new User());
    }

    public Task ChangePasswordAsync(int id, string password)
    {
        var sql = new StringBuilder();
        sql.AppendLine("UPDATE tbl_user ");
        sql.AppendLine("SET PASSWORD = @Password ");
        sql.AppendLine("WHERE ID = @Id;");
        return _dataBaseExecutor.ExecuteAsync(async con =>
            await con.ExecuteAsync(sql.ToString(), new { Id = id, Password = password }));
    }

    public Task<UserResponseWithoutPasswordDTo> UpdateUserProfileAsync(int id, string name)
    {
        var sql = new StringBuilder();
        sql.AppendLine("UPDATE tbl_user         ");
        sql.AppendLine("SET NAME = @Name,       ");
        sql.AppendLine("UPDATED_AT = @UpdatedAt ");
        sql.AppendLine("WHERE ID = @Id;");
        return _dataBaseExecutor.ExecuteAsync(async con =>
        {
            await con.ExecuteAsync(sql.ToString(), new { Id = id, Name = name, UpdatedAt = DateTime.Now });
            var updateUser = await GetUserByIdAsync(id);
            return new UserResponseWithoutPasswordDTo
            {
                Id = updateUser.Id,
                Name = updateUser.Name,
                Email = updateUser.Email,
                Active = updateUser.Active,
                CreatedAt = updateUser.CreatedAt,
                UpdatedAt = updateUser.UpdatedAt
            };
        });
    }

    private static StringBuilder GetRoles()
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT                              ");
        sql.AppendLine("       TR.ID        AS Id          ,");
        sql.AppendLine("       TR.NAME      AS Name        ,");
        sql.AppendLine("       TR.ROLE_TYPE AS RoleType     ");
        sql.AppendLine("FROM tbl_role TR ");
        sql.AppendLine("INNER JOIN tbl_user_role TUR ON (TUR.ID_ROLE = TR.ID)");
        sql.AppendLine("WHERE TUR.ID_USER = @IdUser;");
        return sql;
    }

    private static StringBuilder BuildUserQuery()
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT TU.ID        AS Id         ,");
        sql.AppendLine("       TU.NAME      AS Name       ,");
        sql.AppendLine("       TU.EMAIL     AS Email      ,");
        sql.AppendLine("       TU.ACTIVE    AS Active     ");
        return sql;
    }
    private static StringBuilder GetBaseQuery()
    {
        var sql = BuildUserQuery();
        sql.AppendLine(",       TU.PASSWORD AS Password   ");
        sql.AppendLine(",       TU.CREATED_AT AS CreatedAt ");
        sql.AppendLine(",       TU.UPDATED_AT AS UpdatedAt ");
        sql.AppendLine("FROM tbl_user TU ");
        return sql;
    }
}

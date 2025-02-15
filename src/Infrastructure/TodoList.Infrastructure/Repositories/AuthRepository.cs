using System.Text;
using Dapper;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;

namespace TodoList.Infrastructure.Repositories;

public class AuthRepository:IAuthRepository
{
    private readonly IDatabaseExecutor _dataBaseExecutor;

    public AuthRepository(IDatabaseExecutor dataBaseExecutor)
    {
        _dataBaseExecutor = dataBaseExecutor;
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

        return await _dataBaseExecutor.ExecuteAsync(async con => await con.QueryFirstAsync<int>(sql.ToString(), user));
    }
}

using System.Text;
using Dapper;
using TodoList.Application.DTOs.User;
using TodoList.Application.ports.Repositories;

namespace TodoList.Infrastructure.Repositories
{
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

        public async Task<IEnumerable<UserRolesDTo>> GetUserRolesAsync(int idUser)
        {
             var sql = new StringBuilder();
             sql.AppendLine("SELECT TU.ID        AS Id         ,");
             sql.AppendLine("       TU.NAME      AS Name       ,");
             sql.AppendLine("       TU.EMAIL     AS Email      ,");
             sql.AppendLine("       TU.ACTIVE    AS Active     ,");
             sql.AppendLine("       TR.ID        AS IdRole     ,");
             sql.AppendLine("       TR.NAME      AS RoleName   ,");
             sql.AppendLine("       TR.ROLE_TYPE AS RoleType    ");
             sql.AppendLine("FROM tbl_user TU ");
             sql.AppendLine("INNER JOIN tbl_user_role TUR ON (TUR.ID_USER = TU.ID)");
             sql.AppendLine("INNER JOIN tbl_role TR       ON (TR.ID = TUR.ID_ROLE)");
             sql.AppendLine("WHERE TU.ID = @IdUser;");
             var result = await _dataBaseExecutor.ExecuteAsync(async con =>
                 await con.QueryAsync<UserRolesDTo>(sql.ToString(), new { IdUser = idUser })) ;

             return result;
        }
    }
}

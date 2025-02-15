using System.Text;
using Dapper;
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
    }
}

using System.Text;
using Dapper;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;
using TodoList.Infrastructure.database;

namespace TodoList.Infrastructure.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public TodoRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> CreateAsync(Todo todo)
    {
        todo.CreatedAt = DateTime.Now;
        todo.UpdatedAt = DateTime.Now;
        var sql = new StringBuilder();
        sql.AppendLine("INSERT INTO tbl_todo(");
        sql.AppendLine("       ID_TAG,       ");
        sql.AppendLine("       ID_CATEGORY,  ");
        sql.AppendLine("       TITLE,        ");
        sql.AppendLine("       DESCRIPTION,  ");
        sql.AppendLine("       IS_COMPLETED, ");
        sql.AppendLine("       STATUS,      ");
        sql.AppendLine("       CREATED_AT,   ");
        sql.AppendLine("       UPDATED_AT,   ");
        sql.AppendLine(") VALUES (           ");
        sql.AppendLine("       @IdTag,       ");
        sql.AppendLine("       @IdCategory,  ");
        sql.AppendLine("       @Title,       ");
        sql.AppendLine("       @Description, ");
        sql.AppendLine("       @IsCompleted, ");
        sql.AppendLine("       @CreatedAt,   ");
        sql.AppendLine("       @UpdatedAt    ");
        sql.AppendLine(");                   ");
        sql.AppendLine("SELECT LAST_INSERT_ID();");
        await using var connection = _connectionFactory.Create();
        return await connection.ExecuteAsync(sql.ToString(), todo);
    }

    public async Task<IEnumerable<Todo>> GetAllAsync()
    {
        var sql = GetBaseQuery();
        await using var connection = _connectionFactory.Create();
        return await connection.QueryAsync<Todo>(sql.ToString());
    }

    public async Task<Todo?> GetByIdAsync(int id)
    {
        var sql = GetBaseQuery();
        sql.AppendLine("WHERE ID = @id;");
        await using var connection = _connectionFactory.Create();
        return await connection.QueryFirstOrDefaultAsync<Todo>(sql.ToString(), param: new { id });
    }

    public async Task<int> UpdateAsync(Todo todo)
    {
        todo.UpdatedAt = DateTime.Now;
        var sql = new StringBuilder();
        sql.AppendLine("UPDATE tbl_todo SET                ");
        sql.AppendLine("       TITLE = @Title,             ");
        sql.AppendLine("       DESCRIPTION = @Description, ");
        sql.AppendLine("       IS_COMPLETED = @IsCompleted,");
        sql.AppendLine("       UPDATED_AT = @UpdatedAt     ");
        sql.AppendLine("WHERE ID = @ID;");
        await using var connection = _connectionFactory.Create();
        return await connection.ExecuteAsync(sql.ToString(), param: todo);
    }

    public async Task<int> DeleteAsync(int id)
    {
        var sql = new StringBuilder();
        sql.AppendLine("DELETE FROM tbl_todo");
        sql.AppendLine("WHERE ID = @Id;");
        await using var connection = _connectionFactory.Create();
        return await connection.ExecuteAsync(sql.ToString(), param: new { id });
    }

    private static StringBuilder GetBaseQuery()
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT ID           AS Id,         ");
        sql.AppendLine("       ID_TAG       AS IdTag,      ");
        sql.AppendLine("       ID_CATEGORY  AS IdCategory, ");
        sql.AppendLine("       TITLE        AS Title,      ");
        sql.AppendLine("       DESCRIPTION  AS Description,");
        sql.AppendLine("       IS_COMPLETED AS IsCompleted,");
        sql.AppendLine("       CREATED_AT   AS CreatedAt,  ");
        sql.AppendLine("       UPDATED_AT   AS UpdatedAt   ");
        sql.AppendLine("FROM tbl_todo                      ");
        return sql;
    }
}
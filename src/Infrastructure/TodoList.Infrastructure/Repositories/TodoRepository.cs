using System.Text;
using Dapper;
using TodoList.Application.Ports;
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
    
     public Task<int> CreateAsync(Todo todo)
  {
    todo.CreatedAt = DateTime.Now;
    todo.UpdatedAt = DateTime.Now;
    var sql = new StringBuilder();
    sql.AppendLine("INSERT INTO tbl_todo(");
    sql.AppendLine("       Title,");
    sql.AppendLine("       Description,");
    sql.AppendLine("       IsCompleted,");
    sql.AppendLine("       CreatedAt,");
    sql.AppendLine("       UpdatedAt");
    sql.AppendLine(") VALUES (");
    sql.AppendLine("       @Title,");
    sql.AppendLine("       @Description,");
    sql.AppendLine("       @IsCompleted,");
    sql.AppendLine("       @CreatedAt,");
    sql.AppendLine("       @UpdatedAt");
    sql.AppendLine(");");
    sql.AppendLine("SELECT LAST_INSERT_ID();");
    var connection = _connectionFactory.Create();
    return connection.ExecuteAsync(sql.ToString(), todo);
  }

  public async Task<IEnumerable<Todo>> GetAllAsync()
  {
    var sql = new StringBuilder();
    sql.AppendLine("SELECT ID_Todo,    ");
    sql.AppendLine("       Title,      ");
    sql.AppendLine("       Description,");
    sql.AppendLine("       IsCompleted,");
    sql.AppendLine("       CreatedAt,  ");
    sql.AppendLine("       UpdatedAt   ");
    sql.AppendLine("FROM tbl_todo;     ");
    var connection = _connectionFactory.Create();
    return await connection.QueryAsync<Todo>(sql.ToString());
  }

  public async Task<Todo?> GetByIdAsync(int id)
  {
    var sql = new StringBuilder();
    sql.AppendLine("SELECT ID_Todo,     ");
    sql.AppendLine("       Title,       ");
    sql.AppendLine("       Description, ");
    sql.AppendLine("       IsCompleted, ");
    sql.AppendLine("       CreatedAt,   ");
    sql.AppendLine("       UpdatedAt    ");
    sql.AppendLine("FROM tbl_todo       ");
    sql.AppendLine("WHERE ID_Todo = @id;");
    var connection = _connectionFactory.Create();
    return await connection.QueryFirstOrDefaultAsync<Todo>(sql.ToString(), param: new { id });
  }

  public async Task<int> UpdateAsync(Todo todo)
  {
    todo.UpdatedAt = DateTime.Now;
    var sql = new StringBuilder();
    sql.AppendLine("UPDATE tbl_todo SET");
    sql.AppendLine("       Title = @Title,");
    sql.AppendLine("       Description = @Description,");
    sql.AppendLine("       IsCompleted = @IsCompleted,");
    sql.AppendLine("       UpdatedAt = @UpdatedAt");
    sql.AppendLine("WHERE ID_Todo = @ID_Todo;");
    var connection = _connectionFactory.Create();
    return await connection.ExecuteAsync(sql.ToString(), param:todo);
  }

  public async  Task<int> DeleteAsync(int id)
  {
    var sql = new StringBuilder();
    sql.AppendLine("DELETE FROM tbl_todo");
    sql.AppendLine("WHERE ID_Todo = @id;");
    var connection = _connectionFactory.Create();
    return await connection.ExecuteAsync(sql.ToString(), param: new { id });
  }
}
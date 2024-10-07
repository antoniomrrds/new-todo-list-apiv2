using System.Text;
using Dapper;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;
using TodoList.Infrastructure.database;

namespace TodoList.Infrastructure.Repositories;

public class TagRepository: ITagRepository
{
    private readonly SqlConnectionFactory _connectionFactory;
    
    public TagRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public Task<int> CreateAsync(Tag tag)
    {
        tag.CreatedAt = DateTime.Now;
        tag.UpdatedAt = DateTime.Now;
        var sql = new StringBuilder();
        sql.AppendLine("INSERT INTO tbl_tag(");
        sql.AppendLine("       Name,        ");
        sql.AppendLine("       Color,       ");
        sql.AppendLine("       Description, ");
        sql.AppendLine("       Slug,        ");
        sql.AppendLine("       Status,      ");
        sql.AppendLine("       Created_At,  ");
        sql.AppendLine("       Updated_At   ");
        sql.AppendLine(") VALUES (");
        sql.AppendLine("       @Name,       ");
        sql.AppendLine("       @Color,      ");
        sql.AppendLine("       @Description,");
        sql.AppendLine("       @Slug,       ");
        sql.AppendLine("       @Status,     ");
        sql.AppendLine("       @CreatedAt,  ");
        sql.AppendLine("       @UpdatedAt   ");
        sql.AppendLine(");");
        sql.AppendLine("SELECT LAST_INSERT_ID();");
        var connection = _connectionFactory.Create();
        return connection.ExecuteAsync(sql.ToString(), tag);
    }

    public Task<IEnumerable<Tag>> GetAllAsync()
    {
        var sql = GetBaseQuery();
        var connection = _connectionFactory.Create();
        return connection.QueryAsync<Tag>(sql.ToString());
    }

    public Task<Tag?> GetByIdAsync(int id)
    {
        var sql = GetBaseQuery();
        sql.AppendLine(" WHERE ID = @Id;");
        var connection = _connectionFactory.Create();
        return connection.QueryFirstOrDefaultAsync<Tag>(sql.ToString(), new { Id = id });
    }

    public Task<int> UpdateAsync(Tag tag)
    {
        tag.UpdatedAt = DateTime.Now;
        var sql = new StringBuilder();
        sql.AppendLine("UPDATE tbl_tag                    ");
        sql.AppendLine("   SET Name = @Name,              ");
        sql.AppendLine("       Color = @Color,            ");
        sql.AppendLine("       Description = @Description,");
        sql.AppendLine("       Slug = @Slug,              ");
        sql.AppendLine("       Status = @Status,          ");
        sql.AppendLine("       Updated_At = @UpdatedAt    ");
        sql.AppendLine(" WHERE ID = @Id;                  ");
        var connection = _connectionFactory.Create();
        return connection.ExecuteAsync(sql.ToString(), param: tag);
    }

    public Task<int> DeleteAsync(int id)
    {
        var sql = new StringBuilder();
        sql.AppendLine("DELETE FROM tbl_tag");
        sql.AppendLine(" WHERE ID = @Id;");
        var connection = _connectionFactory.Create();
        return connection.ExecuteAsync(sql.ToString(), new { Id = id });
    }
    
    private static StringBuilder GetBaseQuery()
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT ID,          ");
        sql.AppendLine("       Name,        ");
        sql.AppendLine("       Color,       ");
        sql.AppendLine("       Description, ");
        sql.AppendLine("       Slug,        ");
        sql.AppendLine("       Status,      ");
        sql.AppendLine("       Created_At,  ");
        sql.AppendLine("       Updated_At   ");
        sql.AppendLine("FROM tbl_tag        ");
        return sql;
    }
}
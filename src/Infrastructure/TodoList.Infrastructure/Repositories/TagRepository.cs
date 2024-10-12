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
    
    public async Task<int> CreateAsync(Tag tag)
    {
        tag.CreatedAt = DateTime.Now;
        tag.UpdatedAt = DateTime.Now;
        
        var sql = new StringBuilder();
        sql.AppendLine("INSERT INTO tbl_tag(");
        sql.AppendLine("       NAME,        ");
        sql.AppendLine("       COLOR,       ");
        sql.AppendLine("       DESCRIPTION, ");
        sql.AppendLine("       SLUG,        ");
        sql.AppendLine("       STATUS,      ");
        sql.AppendLine("       CREATED_AT,  ");
        sql.AppendLine("       UPDATED_AT   ");
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
        return await connection.QueryFirstAsync<int>(sql.ToString(), tag);
    }

    public async Task<IEnumerable<Tag>> GetAllTagsWithDetailsAsync()
    {
        var sql = GetBaseQuery();
        using var connection = _connectionFactory.Create();
        var tags = await connection.QueryAsync<Tag>(sql.ToString());
        return tags;
    }

    public async Task<Tag?> GetByIdAsync(int id)
    {
        var sql = GetBaseQuery();
        sql.AppendLine("WHERE ID = @Id;");
        using var connection = _connectionFactory.Create();
        var tag = await connection.QueryFirstOrDefaultAsync<Tag>(sql.ToString(), new { Id = id });
        return tag;
    }

    public async Task<int> UpdateAsync(Tag tag)
    {
        tag.UpdatedAt = DateTime.Now;

        var sql = new StringBuilder();
        sql.AppendLine("UPDATE tbl_tag                    ");
        sql.AppendLine("   SET NAME = @Name,              ");
        sql.AppendLine("       COLOR = @Color,            ");
        sql.AppendLine("       DESCRIPTION = @Description,");
        sql.AppendLine("       SLUG = @Slug,              ");
        sql.AppendLine("       STATUS = @Status,          ");
        sql.AppendLine("       UPDATED_AT = @UpdatedAt    ");
        sql.AppendLine(" WHERE ID = @Id;                  ");

        using var connection = _connectionFactory.Create();
        return await connection.ExecuteAsync(sql.ToString(), tag);
    }

    public  async Task<int> DeleteTagByIdAsync(int id)
    {
        var sql = new StringBuilder();
        sql.AppendLine("DELETE FROM tbl_tag");
        sql.AppendLine(" WHERE ID = @Id;");
        var connection = _connectionFactory.Create();
        return await connection.ExecuteAsync(sql.ToString(), new { Id = id });
    }
    
    private static StringBuilder GetBaseQuery()
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT ID,          ");
        sql.AppendLine("       NAME         AS Name,");
        sql.AppendLine("       COLOR        AS Color,");
        sql.AppendLine("       DESCRIPTION  AS Description,");
        sql.AppendLine("       SLUG         AS Slug,");
        sql.AppendLine("       STATUS       AS Status,");
        sql.AppendLine("       CREATED_AT   AS CreatedAt,");
        sql.AppendLine("       UPDATED_AT   AS UpdatedAt");
        sql.AppendLine("FROM tbl_tag        ");
        return sql;
    }
}
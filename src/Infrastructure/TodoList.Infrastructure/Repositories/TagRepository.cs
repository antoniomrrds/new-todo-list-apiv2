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
        sql.AppendLine("       ACTIVE,      ");
        sql.AppendLine("       CREATED_AT,  ");
        sql.AppendLine("       UPDATED_AT   ");
        sql.AppendLine(") VALUES (");
        sql.AppendLine("       @Name,       ");
        sql.AppendLine("       @Color,      ");
        sql.AppendLine("       @Description,");
        sql.AppendLine("       @Slug,       ");
        sql.AppendLine("       @Active,     ");
        sql.AppendLine("       @CreatedAt,  ");
        sql.AppendLine("       @UpdatedAt   ");
        sql.AppendLine(");");
        sql.AppendLine("SELECT LAST_INSERT_ID();");
        await using var connection = _connectionFactory.Create();
        return await connection.QueryFirstAsync<int>(sql.ToString(), tag);
    }

    public async Task<IEnumerable<Tag>> GetAllTagsWithDetailsAsync()
    {
        var sql = GetBaseQuery();
        await using var connection = _connectionFactory.Create();
        var tags = await connection.QueryAsync<Tag>(sql.ToString());
        return tags;
    }

    public async Task<Tag?> GetByIdAsync(int id)
    {
        var sql = GetBaseQuery();
        sql.AppendLine("WHERE ID = @Id;");
        await using var connection = _connectionFactory.Create();
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
        sql.AppendLine("       ACTIVE = @Active,          ");
        sql.AppendLine("       UPDATED_AT = @UpdatedAt    ");
        sql.AppendLine(" WHERE ID = @Id;                  ");

        await using var connection = _connectionFactory.Create();
        return await connection.ExecuteAsync(sql.ToString(), tag);
    }

    public  async Task<int> DeleteTagByIdAsync(int id)
    {
        var sql = new StringBuilder();
        sql.AppendLine("DELETE FROM tbl_tag");
        sql.AppendLine(" WHERE ID = @Id;");
        await using var connection = _connectionFactory.Create();
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
        sql.AppendLine("       ACTIVE       AS Active,");
        sql.AppendLine("       CREATED_AT   AS CreatedAt,");
        sql.AppendLine("       UPDATED_AT   AS UpdatedAt");
        sql.AppendLine("FROM tbl_tag        ");
        return sql;
    }

    public async Task<bool> AreAllEntitiesPresentAsync(IEnumerable<int> ids)
    {
        var tagIds = ids.ToList();
        if (tagIds.Count == 0) return true;

            const string sql = "SELECT ID FROM tbl_tag WHERE ID IN @TagIds";
             await using var connection = _connectionFactory.Create();
             var existingTagIds = (await connection.QueryAsync<int>(sql, new { TagIds = tagIds })).ToList();

             return existingTagIds.Count == tagIds.Count;
    }
}
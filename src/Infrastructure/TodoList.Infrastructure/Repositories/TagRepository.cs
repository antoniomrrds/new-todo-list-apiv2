using System.Text;
using Dapper;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;

namespace TodoList.Infrastructure.Repositories;

public class TagRepository(IDatabaseExecutor databaseExecutor) : ITagRepository
{
  public async Task<int> CreateAsync(Tag tag)
  {
        tag.SetDateOfCreation();
        var sql = new StringBuilder();
        sql.AppendLine("INSERT INTO tbl_tag(");
        sql.AppendLine("       NAME,        ");
        sql.AppendLine("       DESCRIPTION, ");
        sql.AppendLine("       ACTIVE,      ");
        sql.AppendLine("       CREATED_AT,  ");
        sql.AppendLine("       UPDATED_AT   ");
        sql.AppendLine(") VALUES (");
        sql.AppendLine("       @Name,       ");
        sql.AppendLine("       @Description,");
        sql.AppendLine("       @Active,     ");
        sql.AppendLine("       @CreatedAt,  ");
        sql.AppendLine("       @UpdatedAt   ");
        sql.AppendLine(");");
        sql.AppendLine("SELECT LAST_INSERT_ID();");
        return await databaseExecutor.ExecuteAsync(con => con.ExecuteScalarAsync<int>(sql.ToString(), tag));
    }

    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        var sql = GetBaseQuery();
        return await databaseExecutor.ExecuteAsync(con => con.QueryAsync<Tag>(sql.ToString()));
    }

    public async Task<Tag> GetByIdAsync(int id)
    {
        var sql = GetBaseQuery();
        sql.AppendLine("WHERE ID = @Id;");

        var tag =  await databaseExecutor.ExecuteAsync(con =>
            con.QueryFirstOrDefaultAsync<Tag>(sql.ToString(), new { Id = id }));
        return tag ?? new Tag();
    }

    public async Task<int> UpdateAsync(Tag tag)
    {
        tag.UpdatedAt = DateTime.Now;

        var sql = new StringBuilder();
        sql.AppendLine("UPDATE tbl_tag                    ");
        sql.AppendLine("   SET NAME = @Name,              ");
        sql.AppendLine("       DESCRIPTION = @Description,");
        sql.AppendLine("       ACTIVE = @Active,          ");
        sql.AppendLine("       UPDATED_AT = @UpdatedAt    ");
        sql.AppendLine(" WHERE ID = @Id;                  ");

        return await databaseExecutor.ExecuteAsync(con => con.ExecuteScalarAsync<int>(sql.ToString(), tag));

    }

    public  async Task<int> DeleteTagByIdAsync(int id)
    {
        var sql = new StringBuilder();
        sql.AppendLine("DELETE FROM tbl_tag");
        sql.AppendLine(" WHERE ID = @Id;");
        return await databaseExecutor.ExecuteAsync(con =>
            con.ExecuteAsync(sql.ToString(), new { Id = id }));
    }

    private static StringBuilder GetBaseQuery()
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT ID,          ");
        sql.AppendLine("       NAME         AS Name,");
        sql.AppendLine("       DESCRIPTION  AS Description,");
        sql.AppendLine("       ACTIVE       AS Active,");
        sql.AppendLine("       CREATED_AT   AS CreatedAt,");
        sql.AppendLine("       UPDATED_AT   AS UpdatedAt");
        sql.AppendLine("FROM tbl_tag        ");
        return sql;
    }

    public async Task<IEnumerable<int>> AreAllEntitiesPresentAsync(IEnumerable<int> ids)
    {
        var tagIds = ids.ToList();
        if (tagIds.Count == 0) return new List<int>();

            const string sql = "SELECT ID FROM tbl_tag WHERE ID IN @TagIds";

            var existingTagIds = await databaseExecutor.ExecuteAsync(con =>
                con.QueryAsync<int>(sql, new { TagIds = tagIds }));

             var missingTagIds = tagIds.Except(existingTagIds);

             return missingTagIds;
    }
}

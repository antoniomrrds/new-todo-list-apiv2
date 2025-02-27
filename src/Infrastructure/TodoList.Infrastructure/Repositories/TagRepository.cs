using System.Text;
using Dapper;
using TodoList.Application.DTOs.Tag;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Domain.Extensions;

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

    public async Task<(IEnumerable<Tag> Items, int TotalItems)> FindByFilter(TagFilterDTo filter, int start = 0, int offset = 0)
    {
        var sqlCountPage = new StringBuilder();
        sqlCountPage.AppendLine("SELECT COUNT(*) FROM tbl_tag TG");
        sqlCountPage.AppendLine($"{GetFilteredQuery(filter)};");

        sqlCountPage.AppendLine("-- Fim da consulta COUNT e início da consulta de dados");

        var sql =  GetBaseQuery();
        sql.AppendLine($"{GetFilteredQuery(filter)}");
        sql.AppendLine("ORDER BY TG.ID DESC");
        if (offset > 0)
            sql.AppendLine($"LIMIT {start}, {offset}");
        sqlCountPage.AppendLine(sql.ToString());

        var result = await databaseExecutor.ExecuteAsync(
            async con =>
            {
                await using var multi = await con.QueryMultipleAsync(sqlCountPage.ToString() ,new  { IdUser = filter.IdUser });

                var totalItems = (await multi.ReadAsync<int>()).FirstOrDefault();
                var items = (await multi.ReadAsync<Tag>()).ToList();
                return (items, totalItems);
            });

        return result;
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
        sql.AppendLine("SELECT ID,                         ");
        sql.AppendLine("       NAME         AS Name,       ");
        sql.AppendLine("       DESCRIPTION  AS Description,");
        sql.AppendLine("       ACTIVE       AS Active,     ");
        sql.AppendLine("       CREATED_AT   AS CreatedAt,  ");
        sql.AppendLine("       UPDATED_AT   AS UpdatedAt   ");
        sql.AppendLine("FROM tbl_tag   TG                 ");
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

    private static StringBuilder GetFilteredQuery(TagFilterDTo filter)
    {
        var sql = new StringBuilder();
        if (filter is null) return sql;

        if(!string.IsNullOrWhiteSpace(filter.Name))
        {
            sql.Append($"WHERE TG.NAME LIKE '%{filter.Name}%'");
        }

        if(filter.Active != ActivationState.Unfiltered)
        {
            sql.Append(sql.Length > 0 ? "AND " : "WHERE ");
            sql.AppendLine($" TG.ACTIVE = {filter.Active.ToInt()}");
        }
        return sql;
    }
}

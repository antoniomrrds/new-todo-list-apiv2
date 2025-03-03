using System.Text;
using Dapper;
using TodoList.Application.DTOs.Category;
using TodoList.Application.DTOs.Tag;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Domain.Extensions;
using TodoList.Infrastructure.DataBase;

namespace TodoList.Infrastructure.Repositories;
public class CategoryRepository(IDatabaseExecutor databaseExecutor) : ICategoryRepository
{
    public async Task<int> CreateAsync(Category category)
    {
        category.SetDateOfCreation();

        var sql = new StringBuilder();
        sql.AppendLine("INSERT INTO tbl_category(");
        sql.AppendLine("       NAME,             ");
        sql.AppendLine("       DESCRIPTION,      ");
        sql.AppendLine("       ACTIVE,           ");
        sql.AppendLine("       CREATED_AT,       ");
        sql.AppendLine("       UPDATED_AT        ");
        sql.AppendLine(") VALUES (");
        sql.AppendLine("       @Name,            ");
        sql.AppendLine("       @Description,     ");
        sql.AppendLine("       @Active,          ");
        sql.AppendLine("       @CreatedAt,       ");
        sql.AppendLine("       @UpdatedAt        ");
        sql.AppendLine(");");
        sql.AppendLine("SELECT LAST_INSERT_ID();");
        return await databaseExecutor.ExecuteAsync(async con => await con.QueryFirstAsync<int>(sql.ToString(), category));
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesWithDetailsAsync()
    {
        var sql = GetBaseQuery();
        sql.AppendLine("ORDER BY NAME;");
        return await databaseExecutor.ExecuteAsync(async con => await con.QueryAsync<Category>(sql.ToString()));
    }

    public async Task<Category> GetByIdAsync(int id)
    {
        var sql = GetBaseQuery();
        sql.AppendLine("WHERE ID = @Id;");
        return await databaseExecutor.ExecuteAsync( async con =>  await con.QueryFirstOrDefaultAsync<Category>(sql.ToString(), new { id })) ?? new Category();
    }

    public async Task<(IEnumerable<Category> Items, int TotalItems)> FindByFilter(CategoryFilterDTo filter, int start = 0, int offset = 0)
    {
        var sqlCountPage = new StringBuilder();
        sqlCountPage.AppendLine("SELECT COUNT(*) FROM tbl_category TC");
        sqlCountPage.AppendLine($"{GetFilteredQuery(filter)};");

        sqlCountPage.AppendLine("-- Fim da consulta COUNT e início da consulta de dados");

        var sql =  GetBaseQuery();
        sql.AppendLine($"{GetFilteredQuery(filter)}");
        sql.AppendLine("ORDER BY TC.ID DESC");
        if (offset > 0)
            sql.AppendLine($"LIMIT {start}, {offset}");
        sqlCountPage.AppendLine(sql.ToString());

        var result = await databaseExecutor.ExecuteAsync(
            async con =>
            {
                await using var multi = await con.QueryMultipleAsync(sqlCountPage.ToString() ,new  { IdUser = filter.IdUser });

                var totalItems = (await multi.ReadAsync<int>()).FirstOrDefault();
                var items = (await multi.ReadAsync<Category>()).ToList();
                return (items, totalItems);
            });

        return result;
    }

    public async Task<IEnumerable<int>> AreAllEntitiesPresentAsync(IEnumerable<int> ids)
    {
        var categoriesId = ids.ToList();
        if (categoriesId.Count == 0) return  new List<int>();

        const string sql = "SELECT ID FROM tbl_category WHERE ID IN @CategoryIds";
        var existingCategoriesIds= await databaseExecutor.ExecuteAsync(  con => con.QueryAsync<int>(sql, new { CategoryIds = categoriesId }));
        var missingCategoriesIds = categoriesId.Except(existingCategoriesIds);
        return missingCategoriesIds;
    }

    public async Task<int> UpdateAsync(Category category)
    {
        category.UpdatedAt = DateTime.Now;
        var sql = new StringBuilder();
        sql.AppendLine("UPDATE tbl_category SET            ");
        sql.AppendLine("       NAME = @Name,               ");
        sql.AppendLine("       DESCRIPTION = @Description, ");
        sql.AppendLine("       ACTIVE = @Active,           ");
        sql.AppendLine("       UPDATED_AT = @UpdatedAt     ");
        sql.AppendLine("WHERE ID = @Id;");
        return await databaseExecutor.ExecuteAsync( con =>  con.ExecuteAsync(sql.ToString(), category));
    }

    public async Task<int> DeleteCategoryByIdAsync(int id)
    {
        var sql = new StringBuilder();
        sql.AppendLine("DELETE FROM tbl_category ");
        sql.AppendLine("WHERE ID = @Id;");
        return await databaseExecutor.ExecuteAsync( con =>  con.ExecuteAsync(sql.ToString(), new {  id }));
    }

    private static StringBuilder GetBaseQuery()
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT ID              AS Id,           ");
        sql.AppendLine("       NAME            AS Name,         ");
        sql.AppendLine("       DESCRIPTION     AS Description,  ");
        sql.AppendLine("       ACTIVE          AS Active,       ");
        sql.AppendLine("       CREATED_AT      AS CreatedAt,    ");
        sql.AppendLine("       UPDATED_AT      As UpdatedAt     ");
        sql.AppendLine("FROM tbl_category  TC                   ");
        return sql;
    }

    private static StringBuilder GetFilteredQuery(CategoryFilterDTo filter)
    {
        var sql = new StringBuilder();
        if (filter is null) return sql;

        if(!string.IsNullOrWhiteSpace(filter.Name))
        {
            sql.Append($"WHERE TC.NAME LIKE '%{filter.Name}%'");
        }

        if(filter.Active != ActivationState.Unfiltered)
        {
            sql.Append(sql.Length > 0 ? "AND " : "WHERE ");
            sql.AppendLine($" TC.ACTIVE = {filter.Active.ToInt()}");
        }
        return sql;
    }


}

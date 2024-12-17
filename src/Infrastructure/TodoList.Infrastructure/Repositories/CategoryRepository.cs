using System.Text;
using Dapper;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;
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
        sql.AppendLine("FROM tbl_category                       ");
        return sql;
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
}
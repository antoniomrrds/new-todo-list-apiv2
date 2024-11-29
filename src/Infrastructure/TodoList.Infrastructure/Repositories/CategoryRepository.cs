
using System.Text;
using Dapper;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;
using TodoList.Infrastructure.database;

namespace TodoList.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly SqlConnectionFactory _connectionFactory;

    public CategoryRepository(SqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task<int> CreateAsync(Category category)
    {
        category.CreatedAt = DateTime.Now;
        category.UpdatedAt = DateTime.Now;
        
        var sql = new StringBuilder();
        sql.AppendLine("INSERT INTO tbl_category(");
        sql.AppendLine("       NAME,             ");
        sql.AppendLine("       CREATED_AT,       ");
        sql.AppendLine("       UPDATED_AT        ");   
        sql.AppendLine(") VALUES (");
        sql.AppendLine("       @Name,            ");
        sql.AppendLine("       @CreatedAt,       ");
        sql.AppendLine("       @UpdatedAt        ");
        sql.AppendLine(");");
        sql.AppendLine("SELECT LAST_INSERT_ID();");
        await using var connection = _connectionFactory.Create();
        return await connection.QueryFirstAsync<int>(sql.ToString(), category);
    }
    

    public async Task<IEnumerable<Category>> GetAllCategoriesWithDetailsAsync()
    {
        var sql = GetBaseQuery();
        sql.AppendLine("ORDER BY NAME;");
        await using var connection = _connectionFactory.Create();
        var categories = await connection.QueryAsync<Category>(sql.ToString());
        return categories;
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        var sql = GetBaseQuery();
        sql.AppendLine("WHERE ID = @Id;");
        await using var connection = _connectionFactory.Create();
        var category = await connection.QueryFirstOrDefaultAsync<Category>(sql.ToString(), new { Id = id });
        return category;
    }

    public async Task<int> UpdateAsync(Category category)
    {
        category.UpdatedAt = DateTime.Now;
        var sql = new StringBuilder();
        sql.AppendLine("UPDATE tbl_category SET        ");
        sql.AppendLine("       NAME = @Name,           ");
        sql.AppendLine("       UPDATED_AT = @UpdatedAt ");
        sql.AppendLine("WHERE ID = @Id;");
        await using var connection = _connectionFactory.Create();
        return await connection.ExecuteAsync(sql.ToString(), category);
    }
    
    public async Task<int> DeleteCategoryByIdAsync(int id)
    {
        var sql = new StringBuilder();
        sql.AppendLine("DELETE FROM tbl_category ");
        sql.AppendLine("WHERE ID = @Id;");
        await using var connection = _connectionFactory.Create();
        return await connection.ExecuteAsync(sql.ToString(), new { Id = id });
    }
    
    private static StringBuilder GetBaseQuery()
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT ID,             ");
        sql.AppendLine("       NAME,           ");
        sql.AppendLine("       CREATED_AT,     ");
        sql.AppendLine("       UPDATED_AT      ");
        sql.AppendLine("FROM tbl_category      ");
        return sql;
    }

    public async Task<bool> AreAllEntitiesPresentAsync(IEnumerable<int> ids)
    {
        var categoriesId = ids.ToList();
        if (categoriesId.Count == 0) return true;

        const string sql = "SELECT ID FROM tbl_category WHERE ID IN @CategoryIds";
        await using var connection = _connectionFactory.Create();
        var existingCategoriesIds = (await connection.QueryAsync<int>(sql, new { TagIds = categoriesId })).ToList();

        return existingCategoriesIds.Count == categoriesId.Count;
    }
}
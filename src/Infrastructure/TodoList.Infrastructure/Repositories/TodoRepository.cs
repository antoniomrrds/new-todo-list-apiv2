using System.Data;
using System.Text;
using Dapper;
using TodoList.Application.Constants;
using TodoList.Application.DTOs.Todo;
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

    public async Task<int> CreateAsync(CreateTodoDTo createTodoDTo)
    {
        var todoWithDates = createTodoDTo with
        {
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        var sql = new StringBuilder();
        sql.AppendLine("INSERT INTO tbl_todo(                  ");
        sql.AppendLine("       TITLE,                          ");
        sql.AppendLine("       DESCRIPTION,                    ");
        sql.AppendLine("       IS_COMPLETED,                   ");
        sql.AppendLine("       ACTIVE,                         ");
        sql.AppendLine("       CREATED_AT,                     ");
        sql.AppendLine("       UPDATED_AT,                     ");
        sql.AppendLine("       EXPIRATION_DATE                 ");
        sql.AppendLine(") VALUES (                             ");
        sql.AppendLine("       @Title,                         ");
        sql.AppendLine("       @Description,                   ");
        sql.AppendLine("       @IsCompleted,                   ");
        sql.AppendLine("       @Active,                        ");
        sql.AppendLine("       @CreatedAt,                     ");
        sql.AppendLine("       @UpdatedAt,                     ");
        sql.AppendLine("       @ExpirationDate                 ");
        sql.AppendLine(");                                     ");
        sql.AppendLine("SELECT LAST_INSERT_ID();               ");

        await using var connection = _connectionFactory.Create();
        var taskId = await connection.ExecuteScalarAsync<int>(sql.ToString() ,todoWithDates);

        await InsertTagsAndCategoriesAsync(taskId, todoWithDates.IdTags, todoWithDates.IdCategories, connection);

        return taskId;
    }


    public async Task<IEnumerable<Todo>> GetAllAsync()
    {
        var sql = GetBaseQuery();
        sql.AppendLine("ORDER BY TD.CREATED_AT DESC;");
        await using var connection = _connectionFactory.Create();
        return await connection.QueryAsync<Todo>(sql.ToString());
    }

    
    public async Task<TodoWithTagAndCategoryIdsDto?> GetTodoWithTagAndCategoryIdsAsync(int id)
    {
        var query = GetCombinedQuery();

        await using var connection = _connectionFactory.Create();

        // Executa as consultas em paralelo (simultaneamente)
        await using var multi = await connection.QueryMultipleAsync(query.ToString(), new { TodoId = id, Id = id });

        var todo = await multi.ReadSingleOrDefaultAsync<Todo>();

        var tags = (await multi.ReadAsync<Tag>()).ToList();

        var categories = (await multi.ReadAsync<Category>()).ToList();

        if (todo == null) return null;
        var todoWithTagsAndCategoriesResponse = new TodoWithTagAndCategoryIdsDto(
            todo.Id,
            todo.Title,
            todo.Description,
            todo.IsCompleted,
            todo.Active,
            todo.CreatedAt,
            todo.UpdatedAt,
            todo.ExpirationDate,
            todo.ExpirationDateFormatted,
            todo.CreatedAtFormatted,
            todo.UpdatedAtFormatted,
            tags.Select(t => t.Id).ToList(),
            categories.Select(c => c.Id).ToList()
        );
        return todoWithTagsAndCategoriesResponse;
    }
    
    public async Task<TodoWithTagsAndCategoriesDTo?> GetTodoWithTagsAndCategoriesAsync(int id)
    {
        var query = GetCombinedQuery();

        await using var connection = _connectionFactory.Create();

        // Executa as consultas em paralelo (simultaneamente)
        await using var multi = await connection.QueryMultipleAsync(query.ToString(), new { TodoId = id, Id = id });

        var todo = await multi.ReadSingleOrDefaultAsync<Todo>();

        var tags = (await multi.ReadAsync<Tag>()).ToList();

        var categories = (await multi.ReadAsync<Category>()).ToList();

        if (todo == null) return null;
        var todoWithTagsAndCategoriesResponse = new TodoWithTagsAndCategoriesDTo(
            todo.Id,
            todo.Title,
            todo.Description,
            todo.IsCompleted,
            todo.Active,
            todo.CreatedAt,
            todo.UpdatedAt,
            todo.ExpirationDate,
            todo.ExpirationDateFormatted,
            todo.CreatedAtFormatted,
            todo.UpdatedAtFormatted,
            tags,
            categories
        );
        return todoWithTagsAndCategoriesResponse;
    }

    public async Task<Todo?> GetByIdAsync(int id)
    {
        var sql = GetBaseQuery();
        sql.AppendLine("WHERE t.ID = @id;");
    
        await using var connection = _connectionFactory.Create();
        return await connection.QueryFirstOrDefaultAsync<Todo>(sql.ToString(), new { id });
    }

   public async Task<int> UpdateAsync(UpdateTodoDTo updateTodoDTo)
{
    var updatedTodo = updateTodoDTo with
    {
        UpdatedAt = DateTime.Now
    };

    var updateTodoSql = new StringBuilder();
    updateTodoSql.AppendLine("UPDATE tbl_todo SET                       ");
    updateTodoSql.AppendLine("       TITLE = @Title,                    ");
    updateTodoSql.AppendLine("       DESCRIPTION = @Description,        ");
    updateTodoSql.AppendLine("       IS_COMPLETED = @IsCompleted,       ");
    updateTodoSql.AppendLine("       ACTIVE = @Active,                  ");
    updateTodoSql.AppendLine("       UPDATED_AT = @UpdatedAt,           ");
    updateTodoSql.AppendLine("       EXPIRATION_DATE = @ExpirationDate  ");
    updateTodoSql.AppendLine("WHERE ID = @Id;                           ");

    await using var connection = _connectionFactory.Create();
    var result = await connection.ExecuteAsync(updateTodoSql.ToString(), updatedTodo);

    var existingTags = await GetExistingTags(updateTodoDTo.Id, connection);
    var existingCategories = await GetExistingCategories(updateTodoDTo.Id, connection);

    var tagsToAdd = (updatedTodo.IdTags ?? []).Except(existingTags).ToList();
    var tagsToRemove = existingTags.Except(updatedTodo.IdTags ?? []).ToList();

    if (updatedTodo.IdCategories == null) return result;
    var categoriesToAdd = (updatedTodo.IdCategories ?? []).Except(existingCategories).ToList();
    var categoriesToRemove = existingCategories.Except(updatedTodo.IdCategories ?? []).ToList();

    if (tagsToRemove.Count != 0)
    {
        const string deleteTagsSql = "DELETE FROM tbl_todo_tag WHERE ID_TODO = @Id AND ID_TAG IN @TagsToRemove";
        await connection.ExecuteAsync(deleteTagsSql, new { updatedTodo.Id, TagsToRemove = tagsToRemove });
    }

    if (categoriesToRemove.Count != 0)
    {
        const string deleteCategoriesSql =
            "DELETE FROM tbl_todo_category WHERE ID_TODO = @Id AND ID_CATEGORY IN @CategoriesToRemove";
        await connection.ExecuteAsync(deleteCategoriesSql,
            new { updatedTodo.Id, CategoriesToRemove = categoriesToRemove });
    }

    await InsertTagsAndCategoriesAsync(updatedTodo.Id, tagsToAdd, categoriesToAdd, connection);

    return result;
}


    public async Task<int> DeleteAsync(int id)
    {
        await using var connection = _connectionFactory.Create();
        const string sql = "DELETE FROM tbl_todo WHERE ID = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }


    private async Task InsertTagsAndCategoriesAsync(int taskId, IEnumerable<int>? tags, IEnumerable<int>? categories,
        IDbConnection connection)
    {
        var tagList = tags?.ToList();
        var categoryList = categories?.ToList();

        if (tagList != null && tagList.Count != 0)
        {
            foreach (var tagId in tagList)
            {
                await connection.ExecuteAsync(
                    "INSERT INTO tbl_todo_tag (ID_TODO, ID_TAG) VALUES (@TaskId, @TagId);",
                    new { TaskId = taskId, TagId = tagId });
            }
        }

        if (categoryList != null && categoryList.Count != 0)
        {
            foreach (var categoryId in categoryList)
            {
                await connection.ExecuteAsync(
                    "INSERT INTO tbl_todo_category (ID_TODO, ID_CATEGORY) VALUES (@TaskId, @CategoryId);",
                    new { TaskId = taskId, CategoryId = categoryId });
            }
        }
    }

    private async Task<List<int>> GetExistingTags(int todoId, IDbConnection connection)
    {
        const string sql = "SELECT ID_TAG FROM tbl_todo_tag WHERE ID_TODO = @TodoId";
        var tags = await connection.QueryAsync<int>(sql, new { TodoId = todoId });
        return tags.ToList();
    }


    private async Task<List<int>> GetExistingCategories(int todoId, IDbConnection connection)
    {
        const string sql = "SELECT ID_CATEGORY FROM tbl_todo_category WHERE ID_TODO = @TodoId";
        var categories = await connection.QueryAsync<int>(sql, new { TodoId = todoId });
        return categories.ToList();
    }

    private static StringBuilder GetBaseQuery()
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT TD.ID                                 AS Id,                            ");
        sql.AppendLine("       TD.TITLE                              AS Title,                         ");
        sql.AppendLine("       TD.DESCRIPTION                        AS Description,                   ");
        sql.AppendLine("       TD.IS_COMPLETED                       AS IsCompleted,                   ");
        sql.AppendLine("       TD.EXPIRATION_DATE                    AS ExpirationDate,                ");
        sql.AppendLine("       TD.ACTIVE                             AS Active,                        ");
        sql.AppendLine("       TD.CREATED_AT                         AS CreatedAt,                     ");
        sql.AppendLine("       TD.UPDATED_AT                         AS UpdatedAt,                     ");
        sql.AppendLine("DATE_FORMAT(TD.CREATED_AT, '%d/%m/%Y %H:%i') AS CreatedAtFormatted,            ");
        sql.AppendLine("DATE_FORMAT(TD.UPDATED_AT, '%d/%m/%Y %H:%i') AS UpdatedAtFormatted,            ");
        sql.AppendLine("DATE_FORMAT(TD.EXPIRATION_DATE, '%d/%m/%Y %H:%i') AS ExpirationDateFormatted   ");
        sql.AppendLine("FROM tbl_todo TD                                                               ");
        return sql;
    }

    private static StringBuilder GetTagsQuery()
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT TG.ID          AS Id,                ");
        sql.AppendLine("       TG.NAME        AS Name,              ");
        sql.AppendLine("       TG.COLOR       AS Color,             ");
        sql.AppendLine("       TG.DESCRIPTION AS Description,       ");
        sql.AppendLine("       TG.ACTIVE      AS Active,            ");
        sql.AppendLine("       TG.CREATED_AT  AS CreatedAt,         ");
        sql.AppendLine("       TG.UPDATED_AT  AS UpdatedAt          ");
        sql.AppendLine("FROM tbl_tag TG                             ");
        sql.AppendLine("JOIN tbl_todo_tag TTG ON TTG.ID_TAG = TG.ID ");
        sql.AppendLine("WHERE TTG.ID_TODO = @TodoId                ");
        sql.AppendLine($"AND TG.ACTIVE = {DefaultValues.Active};    ");
        return sql;
    }

    private static StringBuilder GetCategoriesQuery()
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT C.ID          AS Id,                          ");
        sql.AppendLine("       C.NAME        AS Name,                        ");
        sql.AppendLine("       C.DESCRIPTION AS Description,                 ");
        sql.AppendLine("       C.ACTIVE      AS Active,                      ");
        sql.AppendLine("       C.CREATED_AT  AS CreatedAt,                   ");
        sql.AppendLine("       C.UPDATED_AT  AS UpdatedAt                    ");
        sql.AppendLine("FROM tbl_category C                                  ");
        sql.AppendLine("JOIN tbl_todo_category TTC ON TTC.ID_CATEGORY = C.ID  ");
        sql.AppendLine("WHERE TTC.ID_TODO = @TodoId                          ");
        sql.AppendLine($"AND C.ACTIVE = {DefaultValues.Active};              ");
        return sql;
    }

    private static StringBuilder GetCombinedQuery()
    {
        var sql = new StringBuilder();
        sql.Append(GetBaseQuery());
        sql.AppendLine("WHERE TD.ID = @Id");
        sql.AppendLine($"AND TD.ACTIVE = {DefaultValues.Active};");
        sql.AppendLine();
        sql.Append(GetTagsQuery());
        sql.AppendLine();
        sql.Append(GetCategoriesQuery());
        return sql;
    }
}
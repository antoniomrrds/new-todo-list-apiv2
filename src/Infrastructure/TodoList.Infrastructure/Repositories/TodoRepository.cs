using System.Data;
using System.Text;
using Dapper;
using TodoList.Application.DTOs.Todo;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Constants;
using TodoList.Domain.Entities;
using TodoList.Domain.extensions;

namespace TodoList.Infrastructure.Repositories;

public class TodoRepository(IDatabaseExecutor databaseExecutor) : ITodoRepository
{
    public async Task<int> CreateAsync(CreateTodoDTo createTodoDTo)
    {
        createTodoDTo.SetDateOfCreation();

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

        return await databaseExecutor.ExecuteWithTransactionAsync(async (connection, transaction) =>
        {
            var taskId = await connection.ExecuteScalarAsync<int>(sql.ToString(), createTodoDTo);
            await InsertTagsAndCategoriesAsync(taskId, createTodoDTo.IdTags, createTodoDTo.IdCategories, connection,
                transaction);
            return taskId;
        });
    }


    public async Task<IEnumerable<Todo>> GetAllAsync()
    {
        var sql = GetBaseQuery();
        sql.AppendLine("ORDER BY TD.CREATED_AT DESC;");

        return await databaseExecutor.ExecuteAsync<IEnumerable<Todo>>(async
            con => await con.QueryAsync<Todo>(sql.ToString()));
    }

    public async Task<TodoWithTagAndCategoryIdsDto> GetTodoWithTagAndCategoryIdsAsync(int id)
    {
        var query = GetCombinedQuery();

        return await databaseExecutor.ExecuteAsync<TodoWithTagAndCategoryIdsDto>(async connection =>
        {
            await using var multi = await connection.QueryMultipleAsync(query.ToString(), new { TodoId = id, Id = id });

            var todo = await multi.ReadSingleOrDefaultAsync<Todo>();
            var tags = (await multi.ReadAsync<Tag>()).ToList();
            var categories = (await multi.ReadAsync<Category>()).ToList();

            if (todo is null)
            {
                return new TodoWithTagAndCategoryIdsDto();
            }

            var tagAndCategoryIds = new TagAndCategoryFieldsDTo
            {
                IdCategories = categories.Select(c => c.Id).ToList(),
                IdTags = tags.Select(t => t.Id).ToList()
            };

            return new TodoWithTagAndCategoryIdsDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                CompletionStatus = todo.IsCompleted,
                Active = todo.Active,
                CreatedAt = todo.CreatedAt,
                UpdatedAt = todo.UpdatedAt,
                ExpirationDate = todo.ExpirationDate,
                ExpirationDateFormatted = todo.ExpirationDateFormatted,
                CreatedAtFormatted = todo.CreatedAtFormatted,
                UpdatedAtFormatted = todo.UpdatedAtFormatted,
                IdTags = tagAndCategoryIds.IdTags,
                IdCategories = tagAndCategoryIds.IdCategories
            };
        });
    }


    public async Task<TodoWithTagsAndCategoriesDTo> GetTodoWithTagsAndCategoriesAsync(int id)
    {
        var query = GetCombinedQuery();

        return await databaseExecutor.ExecuteAsync<TodoWithTagsAndCategoriesDTo>(async con =>
        {
            await using var multi = await con.QueryMultipleAsync(query.ToString(), new { TodoId = id, Id = id });

            var todo = await multi.ReadSingleOrDefaultAsync<Todo>();
            var tags = (await multi.ReadAsync<Tag>()).ToList();
            var categories = (await multi.ReadAsync<Category>()).ToList();

            if (todo == null) return new TodoWithTagsAndCategoriesDTo();

            return new TodoWithTagsAndCategoriesDTo
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                CompletionStatus = todo.IsCompleted,
                Active = todo.Active,
                CreatedAt = todo.CreatedAt,
                UpdatedAt = todo.UpdatedAt,
                ExpirationDate = todo.ExpirationDate,
                ExpirationDateFormatted = todo.ExpirationDateFormatted,
                CreatedAtFormatted = todo.CreatedAtFormatted,
                UpdatedAtFormatted = todo.UpdatedAtFormatted,
                Tags = tags,
                Categories = categories
            };
        });
    }


    public async Task<Todo> GetByIdAsync(int id)
    {
        var sql = GetBaseQuery();
        sql.AppendLine("WHERE TD.ID = @id;");

        var todo = await databaseExecutor.ExecuteAsync<Todo>(async con =>
            await con.QueryFirstOrDefaultAsync<Todo>(sql.ToString(), new { id }) ?? new Todo());

        return todo;
    }

    public async Task<int> UpdateAsync(UpdateTodoDTo updateTodoDTo)
    {
        updateTodoDTo.UpdatedAt = DateTime.Now;

        var updateTodoSql = new StringBuilder();
        updateTodoSql.AppendLine("UPDATE tbl_todo SET                       ");
        updateTodoSql.AppendLine("       TITLE = @Title,                    ");
        updateTodoSql.AppendLine("       DESCRIPTION = @Description,        ");
        updateTodoSql.AppendLine("       IS_COMPLETED = @IsCompleted,       ");
        updateTodoSql.AppendLine("       ACTIVE = @Active,                   ");
        updateTodoSql.AppendLine("       UPDATED_AT = @UpdatedAt,           ");
        updateTodoSql.AppendLine("       EXPIRATION_DATE = @ExpirationDate  ");
        updateTodoSql.AppendLine("WHERE ID = @Id;                           ");

        return await databaseExecutor.ExecuteWithTransactionAsync(async (connection, transaction) =>
        {
            var result = await connection.ExecuteAsync(updateTodoSql.ToString(), updateTodoDTo, transaction);

            var existingTagsAndCategories = await GetExistingTagsAndCategories(updateTodoDTo.Id, connection);

            var tagsToAdd = updateTodoDTo.IdTags?.Except(existingTagsAndCategories.IdTags ?? []).ToList() ?? [];
            var tagsToRemove = existingTagsAndCategories.IdTags?.Except(updateTodoDTo.IdTags ?? []).ToList() ?? [];

            var categoriesToAdd =
                updateTodoDTo.IdCategories?.Except(existingTagsAndCategories.IdCategories ?? []).ToList() ?? [];
            var categoriesToRemove =
                existingTagsAndCategories.IdCategories?.Except(updateTodoDTo.IdCategories ?? []).ToList() ?? [];

            if (tagsToRemove.Count != 0)
            {
                await DeleteItemsFromRelationshipTableAsync(updateTodoDTo.Id, tagsToRemove, "tbl_todo_tag", "ID_TAG",
                    connection, transaction);
            }

            if (categoriesToRemove.Count != 0)
            {
                await DeleteItemsFromRelationshipTableAsync(updateTodoDTo.Id, categoriesToRemove, "tbl_todo_category",
                    "ID_CATEGORY", connection, transaction);
            }

            await InsertTagsAndCategoriesAsync(updateTodoDTo.Id, tagsToAdd, categoriesToAdd, connection, transaction);

            return result;
        });
    }

    private static async Task<TagAndCategoryFieldsDTo> GetExistingTagsAndCategories(int todoId,
        IDbConnection connection)
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT ID_TAG as IdTag FROM tbl_todo_tag WHERE ID_TODO = @TodoId;");
        sql.AppendLine();
        sql.AppendLine("SELECT ID_CATEGORY AS IdCategory FROM tbl_todo_category WHERE ID_TODO = @TodoId;");

        await using var multi = await connection.QueryMultipleAsync(sql.ToString(), new { TodoId = todoId });

        // Ler múltiplos valores de tags
        var tags = await multi.ReadAsync<int>();

        // Ler múltiplos valores de categorias
        var categories = await multi.ReadAsync<int>();

        return new TagAndCategoryFieldsDTo { IdCategories = categories.ToList(), IdTags = tags.ToList() };
    }


    public async Task<(IEnumerable<Todo> Items, int TotalItems)> FindByFilter(ToDoFilterDTo filter, int start = 0,
        int offset = 0)
    {
        var sqlCountPage = new StringBuilder();
        sqlCountPage.AppendLine("SELECT COUNT(*) FROM tbl_todo TD");
        sqlCountPage.AppendLine($"{GetFilteredQuery(filter)};");

        sqlCountPage.AppendLine("-- Fim da consulta COUNT e início da consulta de dados");

        var sql = GetBaseQuery();
        sql.Append(GetFilteredQuery(filter));
        sql.AppendLine("ORDER BY TD.TITLE DESC");
        if (offset > 0)
            sql.AppendLine($"LIMIT {start}, {offset}");

        sqlCountPage.AppendLine(sql.ToString());

        var result = await databaseExecutor.ExecuteAsync(
            async con =>
            {
                await using var multi = await con.QueryMultipleAsync(sqlCountPage.ToString());

                var totalItems = (await multi.ReadAsync<int>()).FirstOrDefault();
                var items = (await multi.ReadAsync<Todo>()).ToList();
                return (items, totalItems);
            });

        return result;
    }

    private static async Task DeleteItemsFromRelationshipTableAsync(int taskId, IEnumerable<int>? items,
        string tableName, string columnName, IDbConnection connection, IDbTransaction transaction)
    {
        var itemList = items?.ToList();
        if (itemList?.Count != DefaultValues.Inactive)
        {
            var deleteQuery = $"DELETE FROM {tableName} WHERE ID_TODO = @TaskId AND {columnName} IN @Items";
            await connection.ExecuteAsync(deleteQuery, new { TaskId = taskId, Items = itemList }, transaction);
        }
    }

    private static async Task InsertTagsAndCategoriesAsync(int taskId, IEnumerable<int>? tags,
        IEnumerable<int>? categories, IDbConnection connection, IDbTransaction transaction)
    {
        await InsertItemsInBulkMySqlAsync(taskId, tags, "tbl_todo_tag", "ID_TAG", connection, transaction);
        await InsertItemsInBulkMySqlAsync(taskId, categories, "tbl_todo_category", "ID_CATEGORY", connection,
            transaction);
    }


    private static async Task InsertItemsInBulkMySqlAsync(
        int taskId,
        IEnumerable<int>? items,
        string tableName,
        string columnName,
        IDbConnection connection,
        IDbTransaction transaction)
    {
        var itemsList = items?.ToList();
        if (itemsList == null || itemsList.Count == DefaultValues.Inactive)
            return;


        var queryBuilder = new StringBuilder();
        queryBuilder.AppendLine($"INSERT INTO {tableName} (ID_TODO, {columnName}) VALUES ");

        var parameters = new DynamicParameters();

        for (var i = 0; i < itemsList.Count; i++)
        {
            if (i > 0)
                queryBuilder.Append(", ");
            queryBuilder.Append($"(@TaskId{i}, @ItemId{i})");
            parameters.Add($"TaskId{i}", taskId);
            parameters.Add($"ItemId{i}", itemsList[i]);
        }

        queryBuilder.Append(';');

        await connection.ExecuteAsync(queryBuilder.ToString(), parameters, transaction);
    }

    public async Task<int> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM tbl_todo WHERE ID = @Id";

        return await databaseExecutor.ExecuteAsync(async connection =>
            await connection.ExecuteAsync(sql, new { Id = id }));
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
        sql.AppendLine("SELECT TG.ID          AS Id,                 ");
        sql.AppendLine("       TG.NAME        AS Name,               ");
        sql.AppendLine("       TG.COLOR       AS Color,              ");
        sql.AppendLine("       TG.DESCRIPTION AS Description,        ");
        sql.AppendLine("       TG.ACTIVE      AS Active,             ");
        sql.AppendLine("       TG.CREATED_AT  AS CreatedAt,          ");
        sql.AppendLine("       TG.UPDATED_AT  AS UpdatedAt           ");
        sql.AppendLine("FROM tbl_tag TG                              ");
        sql.AppendLine("JOIN tbl_todo_tag TTG ON TTG.ID_TAG = TG.ID  ");
        sql.AppendLine("WHERE TTG.ID_TODO = @TodoId;                 ");
        return sql;
    }

    private static StringBuilder GetCategoriesQuery()
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT C.ID          AS Id,                            ");
        sql.AppendLine("       C.NAME        AS Name,                          ");
        sql.AppendLine("       C.DESCRIPTION AS Description,                   ");
        sql.AppendLine("       C.ACTIVE      AS Active,                        ");
        sql.AppendLine("       C.CREATED_AT  AS CreatedAt,                     ");
        sql.AppendLine("       C.UPDATED_AT  AS UpdatedAt                      ");
        sql.AppendLine("FROM tbl_category C                                    ");
        sql.AppendLine("JOIN tbl_todo_category TTC ON TTC.ID_CATEGORY = C.ID   ");
        sql.AppendLine("WHERE TTC.ID_TODO = @TodoId;                           ");
        return sql;
    }

    private static StringBuilder GetCombinedQuery()
    {
        var sql = new StringBuilder();
        sql.Append(GetBaseQuery());
        sql.AppendLine("WHERE TD.ID = @Id;");
        sql.AppendLine();
        sql.Append(GetTagsQuery());
        sql.AppendLine();
        sql.Append(GetCategoriesQuery());
        return sql;
    }

    private static StringBuilder GetFilteredQuery(ToDoFilterDTo filter)
    {
        var sql = new StringBuilder();
        if (filter is null) return sql;

        if (!string.IsNullOrWhiteSpace(filter.Title))
        {
            sql.Append($"WHERE TD.TITLE LIKE '%{filter.Title}%'");
        }

        if (!filter.IsActive) return sql;
        sql.AppendLine(sql.Length > 0 ? "AND " : "WHERE ");
        sql.Append($" TD.ACTIVE = '{filter.Active.ToInt()}'");
        return sql;
    }
}

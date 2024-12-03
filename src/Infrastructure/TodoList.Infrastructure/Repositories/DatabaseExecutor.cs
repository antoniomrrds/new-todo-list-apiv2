using System.Data;
using TodoList.Infrastructure.DataBase;

namespace TodoList.Infrastructure.Repositories;

public interface IDatabaseExecutor
{
  Task<T> ExecuteAsync<T>(Func<IDbConnection, Task<T>> query);
  Task<T> ExecuteWithTransactionAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> query);
}

public class DatabaseExecutor(IDbConnectionFactory connectionFactory) : IDatabaseExecutor
{
  public async Task<T> ExecuteAsync<T>(Func<IDbConnection, Task<T>> query)
  {
    await using var connection = connectionFactory.Create();
    return await query(connection);
  }

  public async Task<T> ExecuteWithTransactionAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> query)
  {
    await using var connection = connectionFactory.Create();
    await connection.OpenAsync();
    await using var transaction = await connection.BeginTransactionAsync();
    try
    {
      var result = await query(connection, transaction);
      await transaction.CommitAsync();
      return result;
    }
    catch (Exception)
    {
      await transaction.RollbackAsync();
      throw;
    }
  }
}

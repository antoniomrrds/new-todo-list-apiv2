using MySql.Data.MySqlClient;

namespace TodoList.Infrastructure.DataBase;

public interface IDbConnectionFactory
{
    MySqlConnection Create();
}

public class SqlConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public MySqlConnection Create()
    {
        return new MySqlConnection(connectionString);
    }
}
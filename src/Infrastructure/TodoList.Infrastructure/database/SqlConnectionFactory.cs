using MySql.Data.MySqlClient;

namespace TodoList.Infrastructure.database;

public class SqlConnectionFactory
{
    private readonly string _connectionString;
    
    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public MySqlConnection Create()
    {
        return new MySqlConnection(_connectionString);
    }
    
}
namespace TodoList.Application.ports.Repositories;

public interface IDatabaseInitializer
{
    Task InitializeDatabaseAsync();
}

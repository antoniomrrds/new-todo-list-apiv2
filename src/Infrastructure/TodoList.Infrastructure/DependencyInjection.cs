using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Application.ports.Repositories;
using TodoList.Infrastructure.DataBase;
using TodoList.Infrastructure.Repositories;

namespace TodoList.Infrastructure
{
  public static class DependencyInjection
  {
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
      services.AddSingleton<IDbConnectionFactory>(_ =>
      {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
                                         ?? throw new ApplicationException("Connection string 'DefaultConnection' not found.");
        return new SqlConnectionFactory(connectionString);
      });

      services.AddTransient<IDatabaseExecutor, DatabaseExecutor>();


      services.AddScoped<IDbConnection>(serviceProvider =>
      {
        var connectionFactory = serviceProvider.GetRequiredService<IDbConnectionFactory>();
        return connectionFactory.Create();
      });

      services.AddTransient<ITodoRepository, TodoRepository>();
      services.AddTransient<ITagRepository, TagRepository>();
      services.AddTransient<ICategoryRepository, CategoryRepository>();

    }
  }
}
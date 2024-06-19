using System.Data;
using TodoList.Application.Ports;
using TodoList.Infrastructure.database;
using TodoList.Infrastructure.Repositories;

namespace TodoList.Infrastructure
{
    public static class DependencyInjection
    {
        public static void  AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(serviceProvider =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection")
                                       ?? throw new ApplicationException("Connection string 'DefaultConnection' not found.");
                return new SqlConnectionFactory(connectionString);
            });

            services.AddScoped<IDbConnection>(serviceProvider =>
            {
                var connectionFactory = serviceProvider.GetRequiredService<SqlConnectionFactory>();
                return connectionFactory.CreateConnection();
            });
            
            services.AddTransient<ITodoRepository, TodoRepository>();

            return services;
        }
    }
}
using System.Data;
using System.Reflection;
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
            // Validando e configurando a fábrica de conexões
            services.AddSingleton<IDbConnectionFactory>(_ =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                // Garantir que a connection string esteja configurada corretamente
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("A connection string 'DefaultConnection' está ausente ou inválida.");
                }

                return new SqlConnectionFactory(connectionString);
            });

            // Registrando a execução de comandos no banco de dados
            services.AddScoped<IDatabaseExecutor, DatabaseExecutor>();

            // Registrando a conexão com o banco no escopo da requisição
            services.AddScoped<IDbConnection>(serviceProvider =>
            {
                var connectionFactory = serviceProvider.GetRequiredService<IDbConnectionFactory>();
                return connectionFactory.Create();  // Criando a conexão com o banco
            });

           RegisterRepositories(services);
        }

        private static void RegisterRepositories(IServiceCollection services)
        {
            // Obter todas as classes no assembly que implementam a interface IRepository
            var repositoryTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                    .Any(i => i.Name == "I" + t.Name))
                .ToList();

            // Registrar todos os repositórios encontrados
            foreach (var repoType in repositoryTypes)
            {
                var interfaceType = repoType.GetInterfaces().First(i => i.Name == "I" + repoType.Name);
                services.AddScoped(interfaceType, repoType);
            }
        }

    }
}

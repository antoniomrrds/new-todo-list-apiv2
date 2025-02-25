using System.Data;
using System.Reflection;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Application.Ports.Security;
using TodoList.Application.Ports.Storage;
using TodoList.Infrastructure.DataBase;
using TodoList.Infrastructure.Helpers;
using TodoList.Infrastructure.Repositories;
using TodoList.Infrastructure.Security;
using TodoList.Infrastructure.Storage;

namespace TodoList.Infrastructure;

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
                throw new InvalidOperationException(
                    "A connection string 'DefaultConnection' está ausente ou inválida.");
            }

            return new SqlConnectionFactory(connectionString);
        });

        services.AddScoped<IDatabaseExecutor, DatabaseExecutor>();
        services.AddScoped<ITokenGenerator, JwtAdapter>();
        services.AddSingleton<IBlobService , BlobService>();
        services.AddScoped<IHasher<string>, BcryptAdapter>();
        services.AddSingleton(_ => new BlobServiceClient(configuration.GetConnectionString("BlobStorage")));
        services.AddScoped<IDbConnection>(serviceProvider =>
        {
            var connectionFactory = serviceProvider.GetRequiredService<IDbConnectionFactory>();
            return connectionFactory.Create(); // Criando a conexão com o banco
        });
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = Token.GetTokenValidationParameters(configuration);
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var cookie = context.Request.Cookies["token"];
                    if (!string.IsNullOrEmpty(cookie))
                    {
                        context.Token = cookie; // Define o token a ser validado.
                    }

                    return Task.CompletedTask;
                }
            };
        });
        RegisterRepositories(services);
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        var repositoryTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && t.GetInterfaces()
                .Any(i => i.Name == "I" + t.Name))
            .ToList();

        foreach (var repoType in repositoryTypes)
        {
            var interfaceType = repoType.GetInterfaces().First(i => i.Name == "I" + repoType.Name);
            services.AddScoped(interfaceType, repoType);
        }
    }
}

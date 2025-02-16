using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Infrastructure.Repositories
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IDatabaseExecutor _dataBaseExecutor;
        private readonly IRoleRepository _roleRepository;

        public DatabaseInitializer(IDatabaseExecutor dataBaseExecutor, IRoleRepository roleRepository)
        {
            _dataBaseExecutor = dataBaseExecutor;
            _roleRepository = roleRepository;
        }

        public async  Task InitializeDatabaseAsync()
        {
            await _dataBaseExecutor.ExecuteWithTransactionAsync(async (connection, transaction) =>
            {
                // Verificar se o papel "Admin" já existe
                if (!await _roleRepository.RoleExists(Roles.Admin))
                {
                    var adminRole = new Role
                    {
                        Name = "Admin",
                        Description = "Administrador com permissões totais no sistema",
                        RoleType = Roles.Admin,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    await _roleRepository.CreateAsync(adminRole, connection, transaction);
                }

                // Verificar se o papel "User" já existe
                if (await _roleRepository.RoleExists(Roles.User)) return Task.CompletedTask;
                var userRole = new Role
                {
                    Name = "User",
                    Description = "Usuário com permissões limitadas",
                    RoleType = Roles.User,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                await _roleRepository.CreateAsync(userRole, connection, transaction);

                return Task.CompletedTask;
            });
        }

    }
}

using TodoList.Api.Filters;
using TodoList.Api.Middlewares;
using TodoList.Application;
using TodoList.Application.ports.Repositories;
using TodoList.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao contêiner.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddScoped<ValidateModelAttribute>();
// Configura o roteamento para usar URLs e strings de consulta em minúsculas

builder.Services.AddRouting(
    options =>
  {
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
  });

builder.Services.AddCors(options =>
{
  options.AddPolicy("MyPolicy", policy =>
  {
    policy
        .AllowAnyOrigin()       // Permite qualquer origem
        .AllowAnyHeader()       // Permite qualquer cabeçalho
        .AllowAnyMethod()       // Permite qualquer método
        .WithExposedHeaders("Location");  // Expõe o cabeçalho Location
  });
});



builder.Services.AddControllers(options => { options.Filters.Add<ValidateModelAttribute>(); });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    await databaseInitializer.InitializeDatabaseAsync();  // Chama a inicialização do banco de dados
}



// Registra o middleware de tratamento de erros
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configura o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseCors("MyPolicy");

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

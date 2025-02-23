using TodoList.Api.Filters;
using TodoList.Api.Middlewares;
using TodoList.Application;
using TodoList.Application.ports.Repositories;
using TodoList.Infrastructure;

var builder = WebApplication.CreateBuilder(args);





// builder.Services.AddCors(options =>
// {
//   options.AddPolicy("Policy", policy =>
//   {
//     policy
//         .AllowAnyOrigin()       // Permite qualquer origem
//         .AllowAnyHeader()       // Permite qualquer cabeçalho
//         .AllowAnyMethod()       // Permite qualquer método
//         .WithExposedHeaders("Location");  // Expõe o cabeçalho Location
//   });
// });
// Adiciona serviços ao contêiner.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddScoped<ValidateModelAttribute>();
// Configura o roteamento para usar URLs e strings de consulta em minúsculas


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyPolicy",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithExposedHeaders("Location");  // Expõe o cabeçalho Location
        });
});


builder.Services.AddRouting(
    options =>
  {
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
  });



builder.Services.AddControllers(options => { options.Filters.Add<ValidateModelAttribute>(); });
builder.Services.AddAuthorization();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
    await databaseInitializer.InitializeDatabaseAsync();  // Chama a inicialização do banco de dados
}


// app.UseHttpsRedirection();

app.UseCors("MyPolicy");
// Registra o middleware de tratamento de erros
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<TokenRefreshMiddleware>();


// Configura o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}



// Configura o middleware de autenticação no pipeline de requisições HTTP.
// A autenticação é o processo de verificar a identidade de um usuário ou entidade.
// Garante que o usuário é quem ele diz ser, validando credenciais como nome de usuário e senha, tokens, certificados, etc.
app.UseAuthentication();

// Configura o middleware de autorização no pipeline de requisições HTTP.
// A autorização é o processo de verificar se um usuário autenticado tem permissão para acessar um recurso ou realizar uma ação.
// Garante que o usuário tem os direitos necessários para acessar recursos específicos, verificando permissões e roles associadas ao usuário.
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

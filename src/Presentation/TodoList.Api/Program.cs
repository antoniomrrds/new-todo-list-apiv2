using TodoList.Api.Filters;
using TodoList.Api.Middlewares;
using TodoList.Application;
using TodoList.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao contêiner.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddScoped<ValidateModelAttribute>();

builder.Services.AddCors(options => {
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://192.168.18.6:5173", "http://172.17.80.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); 
    });
});

    
    
builder.Services.AddControllers(options => { options.Filters.Add<ValidateModelAttribute>(); });

var app = builder.Build();

// Registra o middleware de tratamento de erros
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configura o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Aplica a política de CORS antes de qualquer outro middleware que manipule requisições
app.UseCors("MyPolicy");

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
    using TodoList.Api.Filters;
    using TodoList.Api.Middlewares;
    using TodoList.Application;
    using TodoList.Infrastructure;

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();
    builder.Services.AddScoped<ValidateModelAttribute>();


    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidateModelAttribute>();
    });


    var app = builder.Build();


    // Register the error handling middleware
    app.UseMiddleware<ErrorHandlingMiddleware>();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Application.DTOs.Todo;
using TodoList.Application.Validators.Todo;


namespace TodoList.Application;

public static class DependencyInjectionApplication{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddValidatorsFromAssembly(typeof(DependencyInjectionApplication).Assembly); 
        services.AddScoped<IValidator<CreateTodoDTo>, TodoDToValidator>();

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
    }
}
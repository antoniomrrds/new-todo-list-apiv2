using System.Diagnostics;
using System.Net;
using System.Text.Json;
using TodoList.Infrastructure.Helpers;

namespace TodoList.Api.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var traceId = Guid.NewGuid().ToString();
        _logger.LogError(ex, "Server failed. TraceId: {TraceId}", traceId);

        context.Response.ContentType = "application/json";

        // Defina o código de status padrão para 500
        var statusCode = (int)HttpStatusCode.InternalServerError;

        // Verifique o tipo de exceção para retornar códigos de status diferentes
        if (ex is CustomHttpException customEx)
        {
            // Se for uma CustomHttpException, utilize o código de status fornecido pela exceção
            statusCode = (int)customEx.StatusCode;
        }
        else if (ex is ArgumentException || ex is InvalidOperationException)
        {
            // Por exemplo, para exceções de argumento, podemos retornar 400
            statusCode = (int)HttpStatusCode.BadRequest;
        }
        else if (ex is KeyNotFoundException)
        {
            // Para exceções de recurso não encontrado, podemos retornar 404
            statusCode = (int)HttpStatusCode.NotFound;
        }

        context.Response.StatusCode = statusCode;

        Debug.Assert(ex.StackTrace != null, "ex.StackTrace != null");
        var errorResponse = _env.IsDevelopment()
            ? new Dictionary<string, object>
            {
                { "traceId", traceId },
                { "status", statusCode },
                { "message", "An unexpected error occurred." },
                { "error", ex.Message },
                { "stackTrace", ex.StackTrace }
            }
            : new Dictionary<string, object>
            {
                { "traceId", traceId },
                { "status", statusCode },
                { "message", "An unexpected error occurred." }
            };

        return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }

}

using System.Net;
using System.Text.Json;
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
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    var errorResponse = _env.IsDevelopment()
    ? new Dictionary<string, object>
    {
        { "traceId", traceId },
        { "status", context.Response.StatusCode },
        { "message", "An unexpected error occurred." },
        { "error", ex.Message },
        { "stackTrace", ex.StackTrace }
    }
    : new Dictionary<string, object>
    {
        { "traceId", traceId },
        { "status", context.Response.StatusCode },
        { "message", "An unexpected error occurred." }
    };

        return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}

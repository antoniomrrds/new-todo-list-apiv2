using System.Net;

namespace TodoList.Infrastructure.Helpers;

public class CustomHttpException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public CustomHttpException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

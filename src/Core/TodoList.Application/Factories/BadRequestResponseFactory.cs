using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TodoList.Application.Factories;

public static class BadRequestResponseFactory
{
    public static BadRequestObjectResult CreateBadRequestResponse(string message , Dictionary<string ,List<string>> errors)
    {
        return new BadRequestObjectResult(new
        {
            success =false,
            statusCode = StatusCodes.Status400BadRequest,
            message,
            errors
        });
    }
}

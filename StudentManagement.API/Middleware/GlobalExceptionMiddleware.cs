using System.Net;
using System.Text.Json;
using StudentManagement.Application.DTOs;
using StudentManagement.Application.Exceptions;

namespace StudentManagement.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during the execution of the request.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = HttpStatusCode.InternalServerError;
        var message = "An internal server error occurred. Please try again later.";
        List<string>? errors = null;

        switch (exception)
        {
            case NotFoundException notFoundEx:
                statusCode = HttpStatusCode.NotFound;
                message = notFoundEx.Message;
                break;

            case BadRequestException badRequestEx:
                statusCode = HttpStatusCode.BadRequest;
                message = badRequestEx.Message;
                break;

            default:
                statusCode = HttpStatusCode.InternalServerError;
                message = "An unexpected error occurred.";
                errors = new List<string> { exception.Message };
                break;
        }

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.ErrorResponse(message, errors);
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var jsonResponse = JsonSerializer.Serialize(response, options);

        return context.Response.WriteAsync(jsonResponse);
    }
}

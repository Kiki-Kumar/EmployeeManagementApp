using EmployeeManagement.Projects.Exceptions;
using EmployeeManagement.Projects.Responses;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Projects.Middleware;

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
        catch (ProjectNotFoundException exception)
        {
            _logger.LogWarning("Project {ProjectId} was not found.", exception.ProjectId);
            await WriteErrorResponseAsync(context, StatusCodes.Status404NotFound, exception.Message);
        }
        catch (ArgumentException exception)
        {
            _logger.LogWarning("Invalid project request: {Message}", exception.Message);
            await WriteErrorResponseAsync(context, StatusCodes.Status400BadRequest, exception.Message);
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "A database error occurred while processing {RequestPath}.", context.Request.Path);
            await WriteErrorResponseAsync(context, StatusCodes.Status500InternalServerError, "A database error occurred. Please try again later.");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unexpected error occurred while processing {RequestPath}.", context.Request.Path);
            await WriteErrorResponseAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new ApiResponse<object>(false, message, null));
    }
}
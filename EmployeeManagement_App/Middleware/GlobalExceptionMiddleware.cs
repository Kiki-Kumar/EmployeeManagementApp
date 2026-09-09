using EmployeeManagementApp.Exceptions;
using EmployeeManagementApp.Responses;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementApp.Middleware;

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
        catch (EmployeeNotFoundException exception)
        {
            _logger.LogWarning("Employee {EmployeeId} was not found.", exception.EmployeeId);
            await WriteErrorResponseAsync(context, StatusCodes.Status404NotFound, "Employee not found.");
        }
        catch (ResourceNotFoundException exception)
        {
            _logger.LogWarning("{ResourceName} {ResourceId} was not found.", exception.ResourceName, exception.ResourceId);
            await WriteErrorResponseAsync(context, StatusCodes.Status404NotFound, exception.Message);
        }
        catch (ResourceConflictException exception)
        {
            _logger.LogWarning("Request conflict: {Message}", exception.Message);
            await WriteErrorResponseAsync(context, StatusCodes.Status409Conflict, exception.Message);
        }
        catch (InvalidLeaveRequestException exception)
        {
            _logger.LogWarning("Invalid leave request: {Message}", exception.Message);
            await WriteErrorResponseAsync(context, StatusCodes.Status400BadRequest, exception.Message);
        }
        catch (InvalidEmployeeReferenceException exception)
        {
            _logger.LogWarning("Invalid employee reference: {Message}", exception.Message);
            await WriteErrorResponseAsync(context, StatusCodes.Status400BadRequest, exception.Message);
        }
        catch (DbUpdateException exception)
        {
            _logger.LogError(exception, "A database error occurred while processing {RequestPath}.", context.Request.Path);
            await WriteErrorResponseAsync(context, StatusCodes.Status500InternalServerError,
                "A database error occurred. Please try again later.");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unexpected error occurred while processing {RequestPath}.", context.Request.Path);
            await WriteErrorResponseAsync(context, StatusCodes.Status500InternalServerError,
                "An unexpected error occurred. Please try again later.");
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new ApiResponse<object>(false, message, null));
    }
}
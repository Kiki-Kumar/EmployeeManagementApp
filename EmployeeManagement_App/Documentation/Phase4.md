# Phase 4: Global Exception Handling and Logging

## What We Built

This phase adds one place to handle unexpected problems and one clear logging setup. Controllers no longer contain repeated error-handling code for a missing employee.

The new files are:

* `Exceptions/EmployeeNotFoundException.cs`
* `Middleware/GlobalExceptionMiddleware.cs`
* `Logging/LoggingConfiguration.cs`

## Custom Exception

`EmployeeNotFoundException` is a custom exception. An exception is an object that represents a problem that prevents normal code from finishing.

```csharp
public class EmployeeNotFoundException : Exception
```

`EmployeeNotFoundException` is this application's descriptive name for the problem.

`Exception` is the built-in .NET base class for errors that can be thrown and caught.

When the service cannot find an employee, it does this:

```csharp
throw new EmployeeNotFoundException(id);
```

`throw` stops the current method and sends the exception upward through the request pipeline. The controller does not need a repeated `if` block or `try/catch` block for every endpoint.

## Global Exception Middleware

Middleware is code in the ASP.NET Core request pipeline. Every HTTP request passes through middleware in the order configured in `Program.cs`.

`GlobalExceptionMiddleware` wraps the rest of the pipeline in a `try/catch`:

```csharp
try
{
    await _next(context);
}
catch (EmployeeNotFoundException exception)
{
    // Create a 404 response.
}
```

`_next(context)` passes the request to the next middleware and eventually to the controller.

`await` waits for that work to finish.

If code below the middleware throws an exception, the matching `catch` block creates the HTTP response.

The middleware handles these cases:

* `EmployeeNotFoundException`: returns `404 Not Found` and `Employee not found.`
* `DbUpdateException`: returns `500 Internal Server Error` and a safe database-error message.
* Any other `Exception`: returns `500 Internal Server Error` and a safe unexpected-error message.

Database and unexpected exceptions are written to the logs with their technical details. The API client receives only a general message, so internal details such as connection settings or stack traces are not exposed.

The exception flow is:

```
EmployeesController
    -> EmployeeService
    -> EmployeeNotFoundException
    -> GlobalExceptionMiddleware
    -> HTTP 404 ApiResponse
```

The middleware is registered early in `Program.cs`:

```csharp
app.UseMiddleware<GlobalExceptionMiddleware>();
```

Putting it early means it can catch exceptions from controllers and middleware that run later in the pipeline.

## Error Response Shape

The middleware uses the existing `ApiResponse<T>` model. A missing employee response is:

```json
{
  "success": false,
  "message": "Employee not found.",
  "data": null
}
```

Validation errors still return `400 Bad Request` through the automatic DTO validation setup from Phase 3. This means invalid requests, missing employees, database problems, and unexpected problems all have meaningful HTTP status codes and a consistent `success`, `message`, and `data` response shape.

## Logging

`ILogger<T>` is ASP.NET Core's built-in logging abstraction. A class asks for a logger in its constructor:

```csharp
private readonly ILogger<EmployeeService> _logger;

public EmployeeService(
    IEmployeeRepository employeeRepository,
    ILogger<EmployeeService> logger)
```

ASP.NET Core dependency injection supplies the logger. `EmployeeService` records useful business events:

```csharp
_logger.LogInformation("Employee creation started for {Email}.", employeeCreateDto.Email);
_logger.LogInformation("Employee {EmployeeId} was created successfully.", createdEmployee.Id);
```

`LogInformation` records normal application activity, such as creating, updating, or deleting an employee.

The global middleware uses `LogWarning` when an employee is not found. A missing employee is an expected situation, so a warning is more appropriate than an error.

The middleware uses `LogError` for database and unexpected exceptions. These log the exception details for the developer while the client gets a safe general message.

The logging flow is:

```
EmployeeService or GlobalExceptionMiddleware
    -> ILogger<T>
    -> LoggingConfiguration
    -> Console and Visual Studio Debug output
```

## Separate Logging Configuration

`Logging/LoggingConfiguration.cs` keeps the logging setup in one focused file:

```csharp
builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();
```

`ClearProviders()` removes default providers so the configured providers are explicit.

`AddConfiguration(...)` reads log levels from the `Logging` section in `appsettings.json`.

`AddConsole()` writes logs to the terminal running the API.

`AddDebug()` writes logs to Visual Studio's Debug output window.

No additional NuGet package is needed because these providers are included with the ASP.NET Core shared framework. A file logger would require an external package such as Serilog, but this beginner project deliberately uses the built-in providers first.

The configured levels in `appsettings.json` are:

```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning"
  }
}
```

`Default: Information` shows application information, warnings, and errors.

`Microsoft.AspNetCore: Warning` reduces routine framework messages while keeping warnings and errors visible.

## How To Run and Test

1. Build and run the API:

```powershell
dotnet build
dotnet run
```

2. Open Swagger at the address printed by the application followed by `/swagger`.
3. Call `GET /api/Employees/999999`. It returns `404 Not Found` with the consistent error response and writes a warning to the terminal.
4. Create an employee with `POST /api/Employees`. The terminal logs the creation start and success at Information level.
5. Try POST with `{}`. It returns `400 Bad Request` with validation details.
6. If SQL Server becomes unavailable or another unexpected exception occurs, the middleware returns `500 Internal Server Error` without exposing the internal exception details. The technical details appear only in the server logs.

## Phase 4 Result

The API now has centralized exception handling and structured built-in logging. CRUD behavior, DTO validation, and Swagger remain in place from earlier phases. Phase 5 will perform final integration checks, review the application, and document the complete project.
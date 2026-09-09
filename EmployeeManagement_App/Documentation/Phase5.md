# Phase 5: Final Integration, Testing, and Project Guide

## Final Result

The Employee Management API is complete. It is a beginner-friendly ASP.NET Core Web API that stores employees in SQL Server using EF Core Code First.

The verified features are:

* SQL Server database creation through EF Core migrations.
* Employee DTO validation.
* Async CRUD endpoints.
* Repository and service layers registered with dependency injection.
* Consistent successful and error responses.
* Global handling for missing employees, database failures, and unexpected exceptions.
* Information, warning, and error logging through `ILogger<T>`.
* Swagger/OpenAPI endpoint discovery and testing.

## Project Architecture

```
Controllers/
    EmployeesController.cs       Receives HTTP requests and sends HTTP responses.
Data/
    ApplicationDbContext.cs      EF Core bridge to SQL Server.
DTOs/
    EmployeeCreateDto.cs         Validated data accepted for POST.
    EmployeeUpdateDto.cs         Validated data accepted for PUT.
    EmployeeResponseDto.cs       Employee data returned to clients.
Exceptions/
    EmployeeNotFoundException.cs Represents a requested employee that does not exist.
Logging/
    LoggingConfiguration.cs      Configures Console and Debug logging.
Middleware/
    GlobalExceptionMiddleware.cs Converts exceptions into safe HTTP responses.
Models/
    Employee.cs                  EF Core entity used for the database table.
Repositories/
    IEmployeeRepository.cs       Database-access contract.
    EmployeeRepository.cs        EF Core database-access implementation.
Responses/
    ApiResponse.cs               Consistent response wrapper.
Services/
    IEmployeeService.cs          Application-operation contract.
    EmployeeService.cs           Mapping and business-operation implementation.
Migrations/                      EF Core schema history.
Documentation/                  Phase-by-phase learning guides.
Program.cs                       Application startup, service registration, and pipeline.
appsettings.json                 Connection string and logging levels.
```

This structure gives each part one clear job. The controller does not directly use EF Core. The repository does not decide HTTP status codes. The middleware handles exceptions in one place.

## Complete Request Flow

For a create request, the flow is:

```
POST /api/employees
    -> EmployeesController
    -> EmployeeService
    -> EmployeeRepository
    -> ApplicationDbContext
    -> SQL Server Employees table
```

1. The client sends JSON to `POST /api/employees`.
2. ASP.NET Core converts the JSON to `EmployeeCreateDto`.
3. Validation attributes check the input. Invalid input returns `400 Bad Request` before the controller method runs.
4. `EmployeesController` calls `IEmployeeService`.
5. `EmployeeService` manually maps the create DTO to an `Employee` entity.
6. `EmployeeRepository` uses `ApplicationDbContext` and EF Core to insert the entity.
7. SQL Server saves the row and creates its numeric `Id`.

Dependency injection connects the parts:

```
EmployeesController
    -> IEmployeeService / EmployeeService
    -> IEmployeeRepository / EmployeeRepository
    -> ApplicationDbContext
    -> SQL Server
```

`AddScoped` is used for the service, repository, and DbContext. A scoped service is created once for each HTTP request, then disposed when that request ends.

## Complete Response and DTO Flow

The response takes this path:

```
SQL Server
    -> ApplicationDbContext
    -> EmployeeRepository
    -> EmployeeService
    -> Employee entity mapped to EmployeeResponseDto
    -> EmployeesController
    -> ApiResponse JSON
```

The `Employee` entity is not sent directly to the client. The service maps it to `EmployeeResponseDto` first. This makes the public API response intentional and avoids exposing a future database-only property by accident.

The response wrapper looks like this:

```json
{
  "success": true,
  "message": "Employee created successfully.",
  "data": {
    "id": 1,
    "firstName": "John",
    "lastName": "Smith",
    "email": "john@example.com",
    "department": "IT",
    "salary": 50000,
    "dateOfJoining": "2026-01-10",
    "isActive": true
  }
}
```

## Database and Migration Flow

EF Core Code First starts with C#:

```
Employee.cs
    -> ApplicationDbContext and DbSet<Employee>
    -> Migration files
    -> SQL Server EmployeeManagementDb database
    -> Employees table
```

The initial migration has already been created and applied. To verify it:

```powershell
dotnet ef migrations list
```

The output includes `InitialCreate` when it is applied.

When you change the `Employee` entity or its EF Core configuration later, create a new migration and apply it:

```powershell
dotnet ef migrations add DescribeYourChange
dotnet ef database update
```

Do not manually change the `Employees` table for model changes. Let the migration describe and apply the schema update.

## Exception Flow

An unknown employee ID follows this path:

```
EmployeesController
    -> EmployeeService
    -> EmployeeNotFoundException
    -> GlobalExceptionMiddleware
    -> HTTP 404 ApiResponse
```

The service throws `EmployeeNotFoundException`. `GlobalExceptionMiddleware` catches it and returns:

```json
{
  "success": false,
  "message": "Employee not found.",
  "data": null
}
```

The middleware also catches `DbUpdateException` and unexpected exceptions. Those return `500 Internal Server Error` with a safe message. The technical details stay in server logs and are never sent to the API client.

## Logging Flow

```
EmployeeService or GlobalExceptionMiddleware
    -> ILogger<T>
    -> LoggingConfiguration
    -> Console and Debug output
```

`EmployeeService` writes Information logs when employee creation, update, and deletion occur.

`GlobalExceptionMiddleware` writes a Warning when an employee is missing.

The middleware writes Error logs for database and unexpected failures.

`Logging/LoggingConfiguration.cs` uses the built-in Console and Debug providers. `appsettings.json` controls which log levels are shown. No file-logging package was added, which keeps the learning project focused on ASP.NET Core's built-in logging.

## Run the Application

1. Ensure SQL Server LocalDB is installed, or update `DefaultConnection` in `appsettings.json` for your SQL Server instance.
2. Restore packages and apply migrations:

```powershell
dotnet restore
dotnet ef database update
```

3. Run with the HTTPS launch profile:

```powershell
dotnet run --launch-profile https
```

4. Open `https://localhost:7145/swagger` in a browser.

Swagger is an interactive API page. It lists each endpoint, request format, and response format. Select an endpoint, choose `Try it out`, enter data, and choose `Execute`.

## Endpoint Testing

Use this request body for POST and PUT:

```json
{
  "firstName": "John",
  "lastName": "Smith",
  "email": "john@example.com",
  "department": "IT",
  "salary": 50000,
  "dateOfJoining": "2026-01-10",
  "isActive": true
}
```

| Action | Endpoint | Expected result |
| --- | --- | --- |
| Get all | `GET /api/employees` | `200 OK` with an array in `data`. |
| Create | `POST /api/employees` | `201 Created` with the created employee in `data`. |
| Get one | `GET /api/employees/{id}` | `200 OK` with the employee in `data`. |
| Update | `PUT /api/employees/{id}` | `200 OK` with the updated employee in `data`. |
| Delete | `DELETE /api/employees/{id}` | `200 OK` with `data: null`. |
| Missing ID | `GET /api/employees/999999` | `404 Not Found` and `Employee not found.` |
| Invalid body | `POST /api/employees` with `{}` | `400 Bad Request` with validation details. |

## Final Verification

The final integration check completed successfully against LocalDB:

* Build completed successfully with `dotnet build`.
* `InitialCreate` was listed as applied by `dotnet ef migrations list`.
* Swagger returned `200 OK`.
* GET all returned `200 OK`.
* POST returned `201 Created`.
* GET by ID, PUT, and DELETE each returned `200 OK`.
* GET by the deleted ID returned `404 Not Found`.
* POST with an empty JSON object returned `400 Bad Request`.
* Information and warning logs were observed for the tested operations.

The temporary employee used for the test was deleted afterward.

## Troubleshooting

### `dotnet ef` is not recognized

Install the matching EF Core command-line tool:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.18
```

### SQL Server or LocalDB connection fails

Check `DefaultConnection` in `appsettings.json`. For SQL Server Express, a common server value is `Server=.\\SQLEXPRESS`. Then run:

```powershell
dotnet ef database update
```

### Migration exists but the table is missing

Run:

```powershell
dotnet ef database update
```

This applies any migration that has not yet been recorded in `__EFMigrationsHistory`.

### Swagger does not open

Run with the Development HTTPS profile:

```powershell
dotnet run --launch-profile https
```

Then open `https://localhost:7145/swagger`. If the browser warns about a development certificate, trust the local .NET development certificate or use the HTTP profile address shown in the terminal.

### Build fails because the executable is in use

Stop any running API process with `Ctrl+C` in its terminal, then run `dotnet build` again.

### API returns `400 Bad Request`

Read the `data` object in the response. It lists each failed DTO validation field. Ensure names, department, and email are supplied; salary is greater than zero; and date of joining is between `1900-01-01` and `2100-12-31`.

### API returns `404 Not Found`

The employee ID does not exist in the database. Create an employee first or use an ID returned by POST or GET all.
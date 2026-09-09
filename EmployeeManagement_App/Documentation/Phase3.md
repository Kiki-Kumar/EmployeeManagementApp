# Phase 3: DTOs, Repository, Service Layer, and CRUD

## What We Built

This phase adds the Employee API endpoints. The API can now create, read, update, and delete employees in the SQL Server database.

```
GET    /api/employees
GET    /api/employees/{id}
POST   /api/employees
PUT    /api/employees/{id}
DELETE /api/employees/{id}
```

The application uses DTOs, a repository, a service, dependency injection, asynchronous database calls, input validation, and consistent API responses.

## DTOs

A DTO is a Data Transfer Object. It is a small class designed for data entering or leaving the API.

* `EmployeeCreateDto` describes the body accepted when creating an employee.
* `EmployeeUpdateDto` describes the body accepted when replacing an employee's details.
* `EmployeeResponseDto` describes the data returned to an API client.

The EF Core `Employee` entity is not returned directly. This keeps the database model separate from the public API contract. A future internal property can be added to `Employee` without accidentally exposing it to API clients.

The service maps data manually:

```
Request JSON
    -> EmployeeCreateDto or EmployeeUpdateDto
    -> Employee entity
    -> SQL Server

SQL Server
    -> Employee entity
    -> EmployeeResponseDto
    -> Response JSON
```

Manual mapping is simple to read in this small project and does not require another library.

## Validation

The create and update DTOs use validation attributes. For example:

```csharp
[Required]
[EmailAddress]
[StringLength(256)]
public string Email { get; set; } = string.Empty;
```

`[Required]` means the client must provide a value.

`[EmailAddress]` checks that the supplied text has an email format.

`[StringLength(256)]` limits the email to 256 characters, matching the database configuration.

Salary must be at least `0.01`, and date of joining must be from `1900-01-01` through `2100-12-31`.

`[ApiController]` on `EmployeesController` makes ASP.NET Core run this validation before the action method. If validation fails, the action does not run and the API sends `400 Bad Request`.

`Program.cs` configures the automatic validation result to use the same response shape as the rest of the API:

```json
{
  "success": false,
  "message": "Validation failed.",
  "data": {
    "Email": ["The Email field is not a valid e-mail address."]
  }
}
```

## Consistent Responses

`Responses/ApiResponse.cs` defines the wrapper used by the controller:

```csharp
public class ApiResponse<T>
{
    public bool Success { get; }
    public string Message { get; }
    public T? Data { get; }
}
```

`T` is a generic type placeholder. It lets the same wrapper return one employee, a list of employees, validation errors, or no data.

A successful employee lookup looks like this:

```json
{
  "success": true,
  "message": "Employee retrieved successfully.",
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

An unknown employee returns `404 Not Found`:

```json
{
  "success": false,
  "message": "Employee not found.",
  "data": null
}
```

## Repository and Service

An interface describes what a class can do without exposing its implementation. `IEmployeeRepository` defines database operations and `EmployeeRepository` implements them with `ApplicationDbContext`.

The repository's job is database access. It uses EF Core calls such as `ToListAsync`, `FirstOrDefaultAsync`, `AddAsync`, and `SaveChangesAsync`.

`IEmployeeService` defines application operations and `EmployeeService` implements them. Its job is to map DTOs, coordinate repository calls, and make controller code small and readable.

`AsNoTracking()` is used when getting all employees. It tells EF Core the returned records are read-only for this request, which avoids change-tracking work.

## Dependency Injection

`Program.cs` registers these classes:

```csharp
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
```

`AddScoped` means one instance is created for each HTTP request. When `EmployeesController` requests `IEmployeeService`, ASP.NET Core creates `EmployeeService`. That service requests `IEmployeeRepository`, so ASP.NET Core supplies `EmployeeRepository`. The repository receives `ApplicationDbContext`.

```
EmployeesController
    -> IEmployeeService / EmployeeService
    -> IEmployeeRepository / EmployeeRepository
    -> ApplicationDbContext
    -> SQL Server
```

Interfaces help classes depend on a contract rather than a specific implementation. This also makes it easier to replace the repository in an automated test later.

## Request and Response Flow

For a request such as `POST /api/employees`:

```
HTTP request JSON
    -> EmployeesController
    -> EmployeeService
    -> EmployeeRepository
    -> ApplicationDbContext
    -> SQL Server
```

The response travels back like this:

```
SQL Server
    -> ApplicationDbContext
    -> EmployeeRepository
    -> EmployeeService
    -> Employee entity mapped to EmployeeResponseDto
    -> EmployeesController
    -> ApiResponse JSON and HTTP response
```

## Async and Await

Database operations can take time. `async` and `await` let the server wait for SQL Server without blocking the request thread. For example:

```csharp
var employees = await _context.Employees.ToListAsync();
```

`ToListAsync()` starts the asynchronous query. `await` pauses this method until the data arrives, while allowing the server to use the thread for other work. The result is then assigned to `employees`.

## HTTP Status Codes

* `200 OK`: successful GET, PUT, and DELETE requests.
* `201 Created`: successful POST. The response includes a `Location` header for the new employee.
* `400 Bad Request`: DTO validation failed.
* `404 Not Found`: no employee has the requested ID.

Unexpected exception handling and logging are deliberately added in Phase 4. The controller currently handles the expected missing-employee case without repetitive `try/catch` blocks.

## Test With Swagger

1. Run the project:

```powershell
dotnet run
```

2. Open the displayed URL with `/swagger`, such as `https://localhost:7001/swagger`.
3. Expand an endpoint, select `Try it out`, enter a value or request body, then select `Execute`.

Use this JSON for POST or PUT:

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

Suggested checks:

* Use POST and confirm `201 Created`.
* Use GET all and find the employee in the `data` array.
* Use GET by ID and confirm `200 OK`.
* Use PUT with the same ID and changed data, then confirm `200 OK`.
* Use DELETE with the ID and confirm `200 OK`.
* Use GET by that deleted ID and confirm `404 Not Found`.
* Try POST with `{}` and confirm `400 Bad Request` and validation errors.

## Verification Completed

This phase was tested against the LocalDB database. Create returned `201`, read returned `200`, update returned `200`, delete returned `200`, and an invalid create returned `400`. The test employee was deleted after verification.
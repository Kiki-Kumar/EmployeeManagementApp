# Phase 1: Project Setup and ASP.NET Core Fundamentals

## What We Built

This phase prepares the Employee Management API project. It configures application startup, controller support, Swagger, application configuration, and the initial `Employee` model. There is no database code or employee CRUD endpoint yet. Those arrive in later phases.

## What Is ASP.NET Core?

ASP.NET Core is a framework for building web applications with C#. A Web API receives HTTP requests, runs C# code, and sends HTTP responses. A future client such as a website, mobile app, or Swagger can call this API.

## Project Structure

```
Controllers/     Future HTTP endpoint classes live here.
Models/          C# classes representing application data.
Documentation/   Beginner-friendly notes for each implementation phase.
Program.cs       Application startup and request-pipeline configuration.
appsettings.json Application settings, including the future database connection string.
```

## Employee Model

`Models/Employee.cs` defines the data the application will manage:

```csharp
public class Employee
{
    public int Id { get; set; }
}
```

`public` means other classes can use this class and property.

`class Employee` creates a blueprint for one employee.

`int` means a whole number. `Id` will become the unique identifier for an employee when Entity Framework Core is introduced in Phase 2.

`{ get; set; }` lets the application read and change the property value.

The remaining properties store names, email, department, salary, joining date, and whether the employee is active. `decimal` is suitable for monetary values. `DateOnly` stores a calendar date without a time of day.

At this stage, this is only a C# model. It is not yet a database table. In Phase 2, the Code First flow will be:

```
Employee C# model
    -> ApplicationDbContext
    -> EF Core migration
    -> SQL Server database
    -> Employee table
```

## Program.cs

`Program.cs` is where ASP.NET Core creates and configures the application.

```csharp
var builder = WebApplication.CreateBuilder(args);
```

This creates a builder. The builder collects services and configuration before the application starts.

```csharp
builder.Services.AddControllers();
```

This registers controller support. A controller is a C# class that will receive API requests such as `GET /api/employees`. The `EmployeesController` is intentionally added in Phase 3, together with its repository and service layers.

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
```

These register Swagger services. Swagger is an interactive web page that lists API endpoints and lets you send test requests without building a separate client.

```csharp
var app = builder.Build();
```

This creates the running application from the builder configuration.

```csharp
app.UseSwagger();
app.UseSwaggerUI();
```

When running in Development, these make the Swagger document and user interface available.

```csharp
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
```

These lines form part of the request pipeline. A request is processed in order: ASP.NET Core can redirect it to HTTPS, apply authorization when it is added later, and route it to a matching controller endpoint. `MapControllers` makes controller routes active.

## Dependency Injection Preview

Dependency injection means ASP.NET Core creates required objects and supplies them to classes that ask for them. For example, a future controller will request `IEmployeeService` in its constructor. ASP.NET Core will provide the registered service implementation. This keeps classes focused on their own job and makes them easier to test.

Phase 1 registers the built-in controller services. Phase 3 will register the employee service and repository dependencies.

## Configuration

`appsettings.json` contains a `DefaultConnection` connection string for the future SQL Server database. Keeping it in configuration, rather than C# code, makes it possible to change database settings without changing application logic. Phase 2 will read this setting to configure Entity Framework Core.

The included value targets SQL Server LocalDB. If LocalDB is unavailable on your computer, replace it in Phase 2 with the server name for your SQL Server instance.

## Run and Test

1. Open a terminal in the project folder.
2. Run:

```powershell
dotnet restore
dotnet run
```

3. The terminal prints one or more `Now listening on:` addresses, for example `https://localhost:7001`.
4. Open that address followed by `/swagger`, for example `https://localhost:7001/swagger`.

Swagger opens successfully, though it has no Employee endpoints in this phase. Employee endpoints are added in Phase 3 after database support is configured in Phase 2.

## Phase 1 Result

The project now has an Employee-focused starting point, working controller and Swagger infrastructure, and configuration ready for the next phase.
# Phase 2: Entity Framework Core, Code First, and SQL Server

## What We Built

This phase connects the application to SQL Server with Entity Framework Core (EF Core). It adds `ApplicationDbContext`, configures the `Employee` model for SQL Server, creates the first migration, and generates the database and `Employees` table.

## What Is Entity Framework Core?

EF Core is a library that helps C# code work with a database. We describe data with C# classes and EF Core creates the required SQL Server tables. Later, it will also read, add, update, and delete records.

## Code First Flow

Code First means the C# code comes first. We do not manually create the `Employees` table in SQL Server.

```
Employee C# model
    -> ApplicationDbContext
    -> EF Core migration
    -> SQL Server database
    -> Employees table
```

`Models/Employee.cs` is the starting point. EF Core recognizes `Id` as the primary key by convention. A primary key uniquely identifies every employee row.

## EF Core Packages

The project now uses these packages:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.18" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.18" />
```

`Microsoft.EntityFrameworkCore.SqlServer` lets EF Core connect to SQL Server.

`Microsoft.EntityFrameworkCore.Design` provides design-time commands, including migration creation. `PrivateAssets` ensures this design-time dependency is not included by projects that reference this API.

## DbContext and DbSet

`Data/ApplicationDbContext.cs` is the bridge between this C# application and SQL Server:

```csharp
public class ApplicationDbContext : DbContext
```

`public` means other classes can use it.

`ApplicationDbContext` is the name of this application's database bridge.

`DbContext` is EF Core's base class. It tracks data changes and knows how to communicate with the configured database.

The constructor receives the database settings:

```csharp
public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
```

`DbContextOptions<ApplicationDbContext>` contains settings such as the database provider and connection string.

`options` arrives through ASP.NET Core dependency injection.

`: base(options)` passes those settings to EF Core.

This line represents a set of employee records:

```csharp
public DbSet<Employee> Employees { get; set; }
```

`DbSet<Employee>` becomes the `Employees` table. In Phase 3, the repository will use this property to query and save employees.

## Entity Configuration

`OnModelCreating` adds database rules for the model. First name, last name, email, and department get sensible maximum lengths. Salary is configured like this:

```csharp
entity.Property(employee => employee.Salary).HasPrecision(18, 2);
```

This makes the SQL Server column `decimal(18,2)`: up to 18 digits total, with 2 digits after the decimal point. This is appropriate for money such as `50000.25`.

## Connection String and Dependency Injection

`appsettings.json` stores the database setting:

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=EmployeeManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

This targets SQL Server LocalDB. The connection string stays in configuration rather than being hard-coded in C#.

`Program.cs` reads and registers it:

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```

`GetConnectionString` reads `DefaultConnection` from the `ConnectionStrings` section.

`AddDbContext` registers the database bridge with dependency injection. It has a scoped lifetime: ASP.NET Core creates one context per HTTP request and disposes it when that request finishes.

`UseSqlServer` selects SQL Server as the EF Core database provider.

## Migration and Database Creation

The following commands were run from the project folder:

```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

`dotnet ef migrations add InitialCreate` created the `Migrations` folder. A migration is a C# record of the database schema changes EF Core needs to make.

`dotnet ef database update` connected to LocalDB, created the `EmployeeManagementDb` database, and applied the migration.

The database now has:

* `Employees`: employee records with `Id`, names, email, department, salary, joining date, and active status.
* `__EFMigrationsHistory`: a table EF Core uses to remember which migrations have run.

When a later model change is needed, create and apply another migration:

```powershell
dotnet ef migrations add DescribeYourChange
dotnet ef database update
```

## How To Run and Verify

1. Build the application:

```powershell
dotnet build
```

2. Check applied migrations:

```powershell
dotnet ef migrations list
```

3. Run the API:

```powershell
dotnet run
```

4. Open the displayed URL followed by `/swagger`, for example `https://localhost:7001/swagger`.

Swagger has no Employee endpoints in this phase. Phase 3 will add the DTOs and CRUD endpoints that use this database.

## Troubleshooting

If `dotnet ef` is not recognized, install its matching tool version:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.18
```

If LocalDB is unavailable, change `DefaultConnection` to your own SQL Server instance. For SQL Server Express, a common server name is `.\\SQLEXPRESS`.

## Phase 2 Result

The Employee model now creates and maps to a SQL Server table through EF Core Code First. The next phase will add DTOs, repository and service layers, validation, and CRUD API endpoints.
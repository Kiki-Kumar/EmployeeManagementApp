# Employee Management Domain Models

## Why More Models?

An employee management system has more information than an employee's name and salary. This application now uses related models so a department, job title, and leave request are represented as real business data.

## Models

### Employee

`Employee` is the main model. It stores personal and employment details, including salary, joining date, and active status.

It now uses `DepartmentId` and `JobTitleId` instead of storing a department as plain text. These are foreign keys. A foreign key connects one table to another table by storing its ID.

An employee can also have many leave requests through the `LeaveRequests` collection.

### Department

`Department` represents an organizational department, for example Human Resources, Information Technology, or Finance.

Each department has a name, optional description, and a collection of employees. One department can have many employees.

### JobTitle

`JobTitle` represents an employee's role, for example Software Developer, HR Specialist, or Accountant.

Each job title has a name, optional description, and a collection of employees. One job title can be assigned to many employees.

### LeaveRequest

`LeaveRequest` represents an employee asking for time away from work. It records the employee ID, start date, end date, reason, and status.

Deleting an employee deletes that employee's leave requests because a leave request cannot exist without its employee.

### LeaveRequestStatus

`LeaveRequestStatus` is an enum, which is a fixed list of allowed values:

```csharp
Pending,
Approved,
Rejected
```

EF Core stores these values as readable text in SQL Server.

## Relationships

```
Department 1 --- many Employees
JobTitle   1 --- many Employees
Employee   1 --- many LeaveRequests
```

Employees cannot be deleted from a Department or JobTitle by accident because the database uses restricted deletion for those relationships. This protects employee records that refer to a department or job title.

## Seeded Reference Data

The migration automatically creates these departments:

| ID | Name |
| --- | --- |
| 1 | Human Resources |
| 2 | Information Technology |
| 3 | Finance |

It also creates these job titles:

| ID | Name |
| --- | --- |
| 1 | Software Developer |
| 2 | HR Specialist |
| 3 | Accountant |

## Create or Update an Employee

Employee POST and PUT requests now use IDs instead of a department text value:

```json
{
  "firstName": "John",
  "lastName": "Smith",
  "email": "john@example.com",
  "departmentId": 2,
  "jobTitleId": 1,
  "salary": 50000,
  "dateOfJoining": "2026-01-10",
  "isActive": true
}
```

The service confirms both IDs exist before saving. An invalid ID returns `400 Bad Request` with a helpful message.

The employee response includes both the IDs and readable names:

```json
{
  "departmentId": 2,
  "departmentName": "Information Technology",
  "jobTitleId": 1,
  "jobTitleName": "Software Developer"
}
```

## Database Migration

The `ExpandEmployeeManagementDomain` migration creates the new tables and relationships. On another machine, run:

```powershell
dotnet ef database update
```

Important: this migration replaces the old text `Department` column with `DepartmentId`. For a production database that already contains employee data, write a data-migration plan before applying it so existing department names can be mapped to Department records.
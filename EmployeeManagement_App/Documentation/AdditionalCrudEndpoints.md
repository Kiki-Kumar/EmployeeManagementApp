# Department, Job Title, and Leave Request CRUD

The Employee Management API now has CRUD endpoints for every current main model: Employee, Department, JobTitle, and LeaveRequest.

## Departments

| Method | Endpoint | Purpose |
| --- | --- | --- |
| GET | `/api/departments` | Get all departments. |
| GET | `/api/departments/{id}` | Get one department. |
| POST | `/api/departments` | Create a department. |
| PUT | `/api/departments/{id}` | Update a department. |
| DELETE | `/api/departments/{id}` | Delete a department without employees. |

Create or update body:

```json
{
  "name": "Operations",
  "description": "Coordinates daily operations."
}
```

Department names must be unique. Deleting a department that still has employees returns `409 Conflict` so employee data remains valid.

## Job Titles

| Method | Endpoint | Purpose |
| --- | --- | --- |
| GET | `/api/jobtitles` | Get all job titles. |
| GET | `/api/jobtitles/{id}` | Get one job title. |
| POST | `/api/jobtitles` | Create a job title. |
| PUT | `/api/jobtitles/{id}` | Update a job title. |
| DELETE | `/api/jobtitles/{id}` | Delete a job title without employees. |

Create or update body:

```json
{
  "name": "Operations Manager",
  "description": "Leads daily operations."
}
```

Job-title names must be unique. A job title assigned to employees cannot be deleted and returns `409 Conflict`.

## Leave Requests

| Method | Endpoint | Purpose |
| --- | --- | --- |
| GET | `/api/leaverequests` | Get all leave requests. |
| GET | `/api/leaverequests/{id}` | Get one leave request. |
| POST | `/api/leaverequests` | Create a leave request. |
| PUT | `/api/leaverequests/{id}` | Update a leave request. |
| DELETE | `/api/leaverequests/{id}` | Delete a leave request. |

Create body:

```json
{
  "employeeId": 1,
  "startDate": "2026-02-01",
  "endDate": "2026-02-03",
  "reason": "Personal leave"
}
```

Update body:

```json
{
  "employeeId": 1,
  "startDate": "2026-02-01",
  "endDate": "2026-02-03",
  "reason": "Personal leave",
  "status": 1
}
```

Leave status values are `0` for Pending, `1` for Approved, and `2` for Rejected. The start and end dates must be valid dates, the employee must exist, and the end date cannot be before the start date. Invalid input returns `400 Bad Request`.

## Response and Error Rules

All endpoints use the existing `ApiResponse` shape. Successful creates return `201 Created`; reads, updates, and deletes return `200 OK`.

Missing resources return `404 Not Found`. Duplicate names and attempts to delete reference data still in use return `409 Conflict`. DTO validation and invalid leave-request dates return `400 Bad Request`.

## Verification

All endpoints were tested against LocalDB. Create, read, update, and delete worked for Department, JobTitle, and LeaveRequest. The test also confirmed `409 Conflict` for a department with an assigned employee and `400 Bad Request` for a leave request whose end date was earlier than its start date. Temporary test data was removed afterward.
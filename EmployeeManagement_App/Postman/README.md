# Employee Management Postman Tests

## Prerequisites

1. Apply the Employee database migrations.
2. Start `EmployeeManagementApp` with IIS Express at `https://localhost:44320`.
3. Start `EmployeeManagementProjects` with IIS Express at `https://localhost:44304` for the employee-projects composition test.
4. Configure the Employee application's Azure Storage connection because employee creation publishes an `EmployeeCreatedEvent` after saving the employee.
5. In Postman settings, disable **SSL certificate verification** for the local IIS Express development certificates if Postman does not trust them.

## Run

1. Import `EmployeeManagement.IISExpress.postman_environment.json`.
2. Import `EmployeeManagementApp.postman_collection.json`.
3. Select the **Employee Management - IIS Express** environment.
4. Run the entire collection in its defined order.

The collection generates unique Department and Job Title names, captures all created IDs, verifies CRUD and error responses, tests the employee links endpoint, exercises Employee-to-Projects HTTP communication, and deletes its generated records.

To run against another IIS server, container, or deployed host, duplicate the environment and change only `baseUrl`. The Projects service URL remains server-side configuration in the Employee application through `ProjectsApi:BaseUrl` or `ProjectsApi__BaseUrl`.

## Important side effect

Employee creation saves the database record before publishing its Azure Queue event. If Azure Storage is unavailable, the API can return `500` after the employee has already been inserted. Ensure Azure Storage is available before running the collection to keep setup and cleanup deterministic.
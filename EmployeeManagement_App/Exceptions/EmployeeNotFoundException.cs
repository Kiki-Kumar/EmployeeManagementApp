namespace EmployeeManagementApp.Exceptions;

public class EmployeeNotFoundException : Exception
{
    public EmployeeNotFoundException(int employeeId)
        : base($"Employee with ID {employeeId} was not found.")
    {
        EmployeeId = employeeId;
    }

    public int EmployeeId { get; }
}
namespace EmployeeManagementApp.Exceptions;

public class ResourceConflictException : Exception
{
    public ResourceConflictException(string message) : base(message)
    {
    }
}
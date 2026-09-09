namespace EmployeeManagementApp.Exceptions;

public class InvalidEmployeeReferenceException : Exception
{
    public InvalidEmployeeReferenceException(string message)
        : base(message)
    {
    }
}
namespace EmployeeManagementApp.Exceptions;

public class InvalidLeaveRequestException : Exception
{
    public InvalidLeaveRequestException(string message) : base(message)
    {
    }
}
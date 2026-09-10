namespace EmployeeManagementApp.Exceptions;

public class ProjectsApiUnavailableException : Exception
{
    public ProjectsApiUnavailableException(string message)
        : base(message)
    {
    }
}

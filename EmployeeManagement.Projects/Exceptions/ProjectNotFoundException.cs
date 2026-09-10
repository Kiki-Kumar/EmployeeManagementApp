namespace EmployeeManagement.Projects.Exceptions;

public class ProjectNotFoundException : Exception
{
    public ProjectNotFoundException(int projectId)
        : base($"Project with ID {projectId} was not found.")
    {
        ProjectId = projectId;
    }

    public int ProjectId { get; }
}

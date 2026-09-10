namespace EmployeeManagement.Projects.Models;

public class Project
{
    public int ProjectId { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public ProjectStatus Status { get; set; } = ProjectStatus.NotStarted;

    public int EmployeeId { get; set; }
}

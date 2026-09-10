namespace EmployeeManagement.Projects.DTOs;

public class ProjectResponseDto
{
    public int ProjectId { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public int EmployeeId { get; set; }
}

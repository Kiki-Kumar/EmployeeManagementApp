namespace EmployeeManagementApp.DTOs;

public class EmployeeLinksResponseDto
{
    public int EmployeeId { get; set; }

    public string EmployeeUrl { get; set; } = string.Empty;

    public string ProjectsUrl { get; set; } = string.Empty;
}
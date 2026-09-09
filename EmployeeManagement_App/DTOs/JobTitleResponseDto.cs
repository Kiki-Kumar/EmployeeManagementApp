namespace EmployeeManagementApp.DTOs;

public class JobTitleResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
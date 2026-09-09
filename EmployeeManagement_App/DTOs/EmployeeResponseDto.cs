namespace EmployeeManagementApp.DTOs;

public class EmployeeResponseDto
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = string.Empty;

    public int JobTitleId { get; set; }

    public string JobTitleName { get; set; } = string.Empty;

    public decimal Salary { get; set; }

    public DateOnly DateOfJoining { get; set; }

    public bool IsActive { get; set; }
}
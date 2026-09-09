namespace EmployeeManagementApp.Models;

public class Employee
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public int DepartmentId { get; set; }

    public Department Department { get; set; } = null!;

    public int JobTitleId { get; set; }

    public JobTitle JobTitle { get; set; } = null!;

    public decimal Salary { get; set; }

    public DateOnly DateOfJoining { get; set; }

    public bool IsActive { get; set; }

    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
}
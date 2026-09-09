using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApp.DTOs;

public class EmployeeUpdateDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int DepartmentId { get; set; }

    [Range(1, int.MaxValue)]
    public int JobTitleId { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999999999")]
    public decimal Salary { get; set; }

    [Range(typeof(DateOnly), "1900-01-01", "2100-12-31")]
    public DateOnly DateOfJoining { get; set; }

    public bool IsActive { get; set; }
}
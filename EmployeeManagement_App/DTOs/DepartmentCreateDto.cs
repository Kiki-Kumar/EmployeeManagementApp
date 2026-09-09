using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApp.DTOs;

public class DepartmentCreateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
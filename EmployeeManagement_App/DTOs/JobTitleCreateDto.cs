using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApp.DTOs;

public class JobTitleCreateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
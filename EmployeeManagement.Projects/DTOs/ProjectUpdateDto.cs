using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Projects.DTOs;

public class ProjectUpdateDto
{
    [Required]
    [StringLength(200)]
    public string ProjectName { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int EmployeeId { get; set; }
}

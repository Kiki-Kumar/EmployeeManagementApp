using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApp.DTOs;

public class LeaveRequestCreateDto
{
    [Range(1, int.MaxValue)]
    public int EmployeeId { get; set; }

    [Range(typeof(DateOnly), "1900-01-01", "2100-12-31")]
    public DateOnly StartDate { get; set; }

    [Range(typeof(DateOnly), "1900-01-01", "2100-12-31")]
    public DateOnly EndDate { get; set; }

    [Required]
    [StringLength(500)]
    public string Reason { get; set; } = string.Empty;
}
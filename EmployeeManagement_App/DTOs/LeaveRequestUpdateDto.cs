using EmployeeManagementApp.Models;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApp.DTOs;

public class LeaveRequestUpdateDto : LeaveRequestCreateDto
{
    [EnumDataType(typeof(LeaveRequestStatus))]
    public LeaveRequestStatus Status { get; set; }
}
using EmployeeManagementApp.DTOs;

namespace EmployeeManagementApp.Services;

public interface ILeaveRequestService
{
    Task<IReadOnlyList<LeaveRequestResponseDto>> GetAllAsync();
    Task<LeaveRequestResponseDto> GetByIdAsync(int id);
    Task<LeaveRequestResponseDto> CreateAsync(LeaveRequestCreateDto leaveRequestCreateDto);
    Task<LeaveRequestResponseDto> UpdateAsync(int id, LeaveRequestUpdateDto leaveRequestUpdateDto);
    Task DeleteAsync(int id);
}
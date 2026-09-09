using EmployeeManagementApp.Models;

namespace EmployeeManagementApp.Repositories;

public interface ILeaveRequestRepository
{
    Task<IReadOnlyList<LeaveRequest>> GetAllAsync();
    Task<LeaveRequest?> GetByIdAsync(int id);
    Task<LeaveRequest> AddAsync(LeaveRequest leaveRequest);
    Task UpdateAsync(LeaveRequest leaveRequest);
    Task DeleteAsync(LeaveRequest leaveRequest);
}
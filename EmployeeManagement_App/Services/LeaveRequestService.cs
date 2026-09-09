using EmployeeManagementApp.DTOs;
using EmployeeManagementApp.Exceptions;
using EmployeeManagementApp.Models;
using EmployeeManagementApp.Repositories;

namespace EmployeeManagementApp.Services;

public class LeaveRequestService : ILeaveRequestService
{
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly IEmployeeRepository _employeeRepository;
    public LeaveRequestService(ILeaveRequestRepository leaveRequestRepository, IEmployeeRepository employeeRepository) { _leaveRequestRepository = leaveRequestRepository; _employeeRepository = employeeRepository; }
    public async Task<IReadOnlyList<LeaveRequestResponseDto>> GetAllAsync() => (await _leaveRequestRepository.GetAllAsync()).Select(MapToResponseDto).ToList();
    public async Task<LeaveRequestResponseDto> GetByIdAsync(int id) => MapToResponseDto(await FindLeaveRequestAsync(id));
    public async Task<LeaveRequestResponseDto> CreateAsync(LeaveRequestCreateDto dto)
    {
        await ValidateAsync(dto.EmployeeId, dto.StartDate, dto.EndDate);
        var leaveRequest = await _leaveRequestRepository.AddAsync(new LeaveRequest { EmployeeId = dto.EmployeeId, StartDate = dto.StartDate, EndDate = dto.EndDate, Reason = dto.Reason });
        return MapToResponseDto(await FindLeaveRequestAsync(leaveRequest.Id));
    }
    public async Task<LeaveRequestResponseDto> UpdateAsync(int id, LeaveRequestUpdateDto dto)
    {
        var leaveRequest = await FindLeaveRequestAsync(id);
        await ValidateAsync(dto.EmployeeId, dto.StartDate, dto.EndDate);
        leaveRequest.EmployeeId = dto.EmployeeId;
        leaveRequest.StartDate = dto.StartDate;
        leaveRequest.EndDate = dto.EndDate;
        leaveRequest.Reason = dto.Reason;
        leaveRequest.Status = dto.Status;
        await _leaveRequestRepository.UpdateAsync(leaveRequest);
        return MapToResponseDto(leaveRequest);
    }
    public async Task DeleteAsync(int id) { await _leaveRequestRepository.DeleteAsync(await FindLeaveRequestAsync(id)); }
    private async Task<LeaveRequest> FindLeaveRequestAsync(int id) => await _leaveRequestRepository.GetByIdAsync(id) ?? throw new ResourceNotFoundException("Leave request", id);
    private async Task ValidateAsync(int employeeId, DateOnly startDate, DateOnly endDate)
    {
        if (endDate < startDate) throw new InvalidLeaveRequestException("End date cannot be earlier than the start date.");
        if (await _employeeRepository.GetByIdAsync(employeeId) is null) throw new EmployeeNotFoundException(employeeId);
    }
    private static LeaveRequestResponseDto MapToResponseDto(LeaveRequest leaveRequest) => new() { Id = leaveRequest.Id, EmployeeId = leaveRequest.EmployeeId, EmployeeName = $"{leaveRequest.Employee.FirstName} {leaveRequest.Employee.LastName}", StartDate = leaveRequest.StartDate, EndDate = leaveRequest.EndDate, Reason = leaveRequest.Reason, Status = leaveRequest.Status };
}
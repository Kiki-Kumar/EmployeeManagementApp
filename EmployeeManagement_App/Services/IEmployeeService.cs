using EmployeeManagementApp.DTOs;

namespace EmployeeManagementApp.Services;

public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeeResponseDto>> GetAllAsync();

    Task<EmployeeResponseDto> GetByIdAsync(int id);

    Task<IReadOnlyList<ProjectResponseDto>> GetProjectsAsync(int employeeId);

    Task<EmployeeResponseDto> CreateAsync(EmployeeCreateDto employeeCreateDto);

    Task<EmployeeResponseDto> UpdateAsync(int id, EmployeeUpdateDto employeeUpdateDto);

    Task DeleteAsync(int id);
}
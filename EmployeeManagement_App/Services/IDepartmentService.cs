using EmployeeManagementApp.DTOs;

namespace EmployeeManagementApp.Services;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentResponseDto>> GetAllAsync();
    Task<DepartmentResponseDto> GetByIdAsync(int id);
    Task<DepartmentResponseDto> CreateAsync(DepartmentCreateDto departmentCreateDto);
    Task<DepartmentResponseDto> UpdateAsync(int id, DepartmentUpdateDto departmentUpdateDto);
    Task DeleteAsync(int id);
}
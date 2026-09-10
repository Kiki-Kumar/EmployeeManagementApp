using EmployeeManagement.Projects.DTOs;

namespace EmployeeManagement.Projects.Services;

public interface IProjectService
{
    Task<IReadOnlyList<ProjectResponseDto>> GetAllAsync();

    Task<ProjectResponseDto> GetByIdAsync(int projectId);

    Task<IReadOnlyList<ProjectResponseDto>> GetByEmployeeIdAsync(int employeeId);

    Task<ProjectResponseDto> CreateAsync(ProjectCreateDto dto);

    Task<ProjectResponseDto> UpdateAsync(int projectId, ProjectUpdateDto dto);

    Task DeleteAsync(int projectId);
}

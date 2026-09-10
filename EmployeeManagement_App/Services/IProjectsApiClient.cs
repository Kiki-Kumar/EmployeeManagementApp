using EmployeeManagementApp.DTOs;

namespace EmployeeManagementApp.Services;

public interface IProjectsApiClient
{
    Task<IReadOnlyList<ProjectResponseDto>> GetProjectsByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default);
}

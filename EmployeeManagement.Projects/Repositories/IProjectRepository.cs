using EmployeeManagement.Projects.Models;

namespace EmployeeManagement.Projects.Repositories;

public interface IProjectRepository
{
    Task<IReadOnlyList<Project>> GetAllAsync();

    Task<Project?> GetByIdAsync(int projectId);

    Task<IReadOnlyList<Project>> GetByEmployeeIdAsync(int employeeId);

    Task<Project> AddAsync(Project project);

    Task UpdateAsync(Project project);

    Task DeleteAsync(Project project);
}

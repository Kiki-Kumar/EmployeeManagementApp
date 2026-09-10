using EmployeeManagement.Projects.DTOs;
using EmployeeManagement.Projects.Exceptions;
using EmployeeManagement.Projects.Models;
using EmployeeManagement.Projects.Repositories;

namespace EmployeeManagement.Projects.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(IProjectRepository projectRepository, ILogger<ProjectService> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ProjectResponseDto>> GetAllAsync()
    {
        var projects = await _projectRepository.GetAllAsync();
        _logger.LogInformation("Retrieved all projects.");
        return projects.Select(MapToResponseDto).ToList();
    }

    public async Task<ProjectResponseDto> GetByIdAsync(int projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId)
            ?? throw new ProjectNotFoundException(projectId);

        _logger.LogInformation("Retrieved project {ProjectId}.", projectId);
        return MapToResponseDto(project);
    }

    public async Task<IReadOnlyList<ProjectResponseDto>> GetByEmployeeIdAsync(int employeeId)
    {
        var projects = await _projectRepository.GetByEmployeeIdAsync(employeeId);
        _logger.LogInformation("Retrieved {ProjectCount} projects for employee {EmployeeId}.", projects.Count, employeeId);
        return projects.Select(MapToResponseDto).ToList();
    }

    public async Task<ProjectResponseDto> CreateAsync(ProjectCreateDto dto)
    {
        ValidateProject(dto.StartDate, dto.EndDate, dto.Status, dto.EmployeeId, dto.ProjectName);

        var project = new Project
        {
            ProjectName = dto.ProjectName,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = ParseStatus(dto.Status),
            EmployeeId = dto.EmployeeId
        };

        var createdProject = await _projectRepository.AddAsync(project);
        _logger.LogInformation("Project {ProjectId} created for employee {EmployeeId}.", createdProject.ProjectId, createdProject.EmployeeId);
        return MapToResponseDto(createdProject);
    }

    public async Task<ProjectResponseDto> UpdateAsync(int projectId, ProjectUpdateDto dto)
    {
        var project = await _projectRepository.GetByIdAsync(projectId)
            ?? throw new ProjectNotFoundException(projectId);

        ValidateProject(dto.StartDate, dto.EndDate, dto.Status, dto.EmployeeId, dto.ProjectName);

        project.ProjectName = dto.ProjectName;
        project.Description = dto.Description;
        project.StartDate = dto.StartDate;
        project.EndDate = dto.EndDate;
        project.Status = ParseStatus(dto.Status);
        project.EmployeeId = dto.EmployeeId;

        await _projectRepository.UpdateAsync(project);

        _logger.LogInformation("Project {ProjectId} updated for employee {EmployeeId}.", projectId, dto.EmployeeId);
        return MapToResponseDto(project);
    }

    public async Task DeleteAsync(int projectId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId)
            ?? throw new ProjectNotFoundException(projectId);

        await _projectRepository.DeleteAsync(project);
        _logger.LogInformation("Project {ProjectId} deleted.", projectId);
    }

    private static ProjectResponseDto MapToResponseDto(Project project)
    {
        return new ProjectResponseDto
        {
            ProjectId = project.ProjectId,
            ProjectName = project.ProjectName,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Status = project.Status.ToString(),
            EmployeeId = project.EmployeeId
        };
    }

    private static void ValidateProject(DateOnly startDate, DateOnly endDate, string status, int employeeId, string projectName)
    {
        if (string.IsNullOrWhiteSpace(projectName))
        {
            throw new ArgumentException("Project name is required.");
        }

        if (employeeId <= 0)
        {
            throw new ArgumentException("EmployeeId must be a positive integer.");
        }

        if (startDate > endDate)
        {
            throw new ArgumentException("EndDate cannot be earlier than StartDate.");
        }

        if (!Enum.TryParse<ProjectStatus>(status, ignoreCase: true, out _))
        {
            throw new ArgumentException("The supplied project status is invalid.");
        }
    }

    private static ProjectStatus ParseStatus(string status)
    {
        return Enum.Parse<ProjectStatus>(status, ignoreCase: true);
    }
}

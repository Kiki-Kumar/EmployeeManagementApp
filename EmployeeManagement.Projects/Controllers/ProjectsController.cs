using EmployeeManagement.Projects.DTOs;
using EmployeeManagement.Projects.Responses;
using EmployeeManagement.Projects.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Projects.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ProjectResponseDto>>>> GetAll()
    {
        var projects = await _projectService.GetAllAsync();
        return Ok(new ApiResponse<IReadOnlyList<ProjectResponseDto>>(true, "Projects retrieved successfully.", projects));
    }

    [HttpGet("{projectId:int}")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> GetById(int projectId)
    {
        var project = await _projectService.GetByIdAsync(projectId);
        return Ok(new ApiResponse<ProjectResponseDto>(true, "Project retrieved successfully.", project));
    }

    [HttpGet("employee/{employeeId:int}")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ProjectResponseDto>>>> GetByEmployeeId(int employeeId)
    {
        var projects = await _projectService.GetByEmployeeIdAsync(employeeId);
        return Ok(new ApiResponse<IReadOnlyList<ProjectResponseDto>>(true, "Projects for employee retrieved successfully.", projects));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Create(ProjectCreateDto dto)
    {
        var project = await _projectService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { projectId = project.ProjectId }, new ApiResponse<ProjectResponseDto>(true, "Project created successfully.", project));
    }

    [HttpPut("{projectId:int}")]
    public async Task<ActionResult<ApiResponse<ProjectResponseDto>>> Update(int projectId, ProjectUpdateDto dto)
    {
        var project = await _projectService.UpdateAsync(projectId, dto);
        return Ok(new ApiResponse<ProjectResponseDto>(true, "Project updated successfully.", project));
    }

    [HttpDelete("{projectId:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int projectId)
    {
        await _projectService.DeleteAsync(projectId);
        return Ok(new ApiResponse<object>(true, "Project deleted successfully.", null));
    }
}

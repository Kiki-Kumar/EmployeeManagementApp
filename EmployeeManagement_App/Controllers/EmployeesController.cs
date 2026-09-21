using EmployeeManagementApp.DTOs;
using EmployeeManagementApp.Responses;
using EmployeeManagementApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IProjectsApiClient _projectsApiClient;

    public EmployeesController(IEmployeeService employeeService, IProjectsApiClient projectsApiClient)
    {
        _employeeService = employeeService;
        _projectsApiClient = projectsApiClient;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<EmployeeResponseDto>>>> GetAll()
    {
        var employees = await _employeeService.GetAllAsync();
        return Ok(new ApiResponse<IReadOnlyList<EmployeeResponseDto>>(
            true, "Employees retrieved successfully.", employees));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<EmployeeResponseDto>>> GetById(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        return Ok(new ApiResponse<EmployeeResponseDto>(true, "Employee retrieved successfully.", employee));
    }

    [HttpGet("{id:int}/links")]
    public async Task<ActionResult<ApiResponse<EmployeeLinksResponseDto>>> GetLinks(int id)
    {
        await _employeeService.GetByIdAsync(id);

        var employeeUrl = Url.ActionLink(nameof(GetById), values: new { id });
        var projectsUrl = Url.ActionLink(nameof(GetProjectsByEmployeeId), values: new { employeeId = id });

        if (employeeUrl is null || projectsUrl is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        var links = new EmployeeLinksResponseDto
        {
            EmployeeId = id,
            EmployeeUrl = employeeUrl,
            ProjectsUrl = projectsUrl
        };

        return Ok(new ApiResponse<EmployeeLinksResponseDto>(true, "Employee links retrieved successfully.", links));
    }

    [HttpGet("{employeeId:int}/projects")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ProjectResponseDto>>>> GetProjectsByEmployeeId(int employeeId)
    {
        await _employeeService.GetByIdAsync(employeeId);
        var projects = await _projectsApiClient.GetProjectsByEmployeeIdAsync(employeeId);
        return Ok(new ApiResponse<IReadOnlyList<ProjectResponseDto>>(true, "Employee projects retrieved successfully.", projects));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<EmployeeResponseDto>>> Create(EmployeeCreateDto employeeCreateDto)
    {
        var employee = await _employeeService.CreateAsync(employeeCreateDto);
        var response = new ApiResponse<EmployeeResponseDto>(true, "Employee created successfully.", employee);
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<EmployeeResponseDto>>> Update(int id, EmployeeUpdateDto employeeUpdateDto)
    {
        var employee = await _employeeService.UpdateAsync(id, employeeUpdateDto);
        return Ok(new ApiResponse<EmployeeResponseDto>(true, "Employee updated successfully.", employee));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _employeeService.DeleteAsync(id);
        return Ok(new ApiResponse<object>(true, "Employee deleted successfully.", null));
    }
}
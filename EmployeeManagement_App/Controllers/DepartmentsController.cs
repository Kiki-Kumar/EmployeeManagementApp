using EmployeeManagementApp.DTOs;
using EmployeeManagementApp.Responses;
using EmployeeManagementApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;
    public DepartmentsController(IDepartmentService departmentService) => _departmentService = departmentService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<DepartmentResponseDto>>>> GetAll() => Ok(new ApiResponse<IReadOnlyList<DepartmentResponseDto>>(true, "Departments retrieved successfully.", await _departmentService.GetAllAsync()));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<DepartmentResponseDto>>> GetById(int id) => Ok(new ApiResponse<DepartmentResponseDto>(true, "Department retrieved successfully.", await _departmentService.GetByIdAsync(id)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DepartmentResponseDto>>> Create(DepartmentCreateDto dto)
    {
        var department = await _departmentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = department.Id }, new ApiResponse<DepartmentResponseDto>(true, "Department created successfully.", department));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<DepartmentResponseDto>>> Update(int id, DepartmentUpdateDto dto) => Ok(new ApiResponse<DepartmentResponseDto>(true, "Department updated successfully.", await _departmentService.UpdateAsync(id, dto)));

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id) { await _departmentService.DeleteAsync(id); return Ok(new ApiResponse<object>(true, "Department deleted successfully.", null)); }
}
using EmployeeManagementApp.DTOs;
using EmployeeManagementApp.Responses;
using EmployeeManagementApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobTitlesController : ControllerBase
{
    private readonly IJobTitleService _jobTitleService;
    public JobTitlesController(IJobTitleService jobTitleService) => _jobTitleService = jobTitleService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<JobTitleResponseDto>>>> GetAll() => Ok(new ApiResponse<IReadOnlyList<JobTitleResponseDto>>(true, "Job titles retrieved successfully.", await _jobTitleService.GetAllAsync()));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<JobTitleResponseDto>>> GetById(int id) => Ok(new ApiResponse<JobTitleResponseDto>(true, "Job title retrieved successfully.", await _jobTitleService.GetByIdAsync(id)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<JobTitleResponseDto>>> Create(JobTitleCreateDto dto)
    {
        var jobTitle = await _jobTitleService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = jobTitle.Id }, new ApiResponse<JobTitleResponseDto>(true, "Job title created successfully.", jobTitle));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<JobTitleResponseDto>>> Update(int id, JobTitleUpdateDto dto) => Ok(new ApiResponse<JobTitleResponseDto>(true, "Job title updated successfully.", await _jobTitleService.UpdateAsync(id, dto)));

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id) { await _jobTitleService.DeleteAsync(id); return Ok(new ApiResponse<object>(true, "Job title deleted successfully.", null)); }
}
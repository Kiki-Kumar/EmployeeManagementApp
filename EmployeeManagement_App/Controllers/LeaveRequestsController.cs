using EmployeeManagementApp.DTOs;
using EmployeeManagementApp.Responses;
using EmployeeManagementApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestService _leaveRequestService;
    public LeaveRequestsController(ILeaveRequestService leaveRequestService) => _leaveRequestService = leaveRequestService;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<LeaveRequestResponseDto>>>> GetAll() => Ok(new ApiResponse<IReadOnlyList<LeaveRequestResponseDto>>(true, "Leave requests retrieved successfully.", await _leaveRequestService.GetAllAsync()));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<LeaveRequestResponseDto>>> GetById(int id) => Ok(new ApiResponse<LeaveRequestResponseDto>(true, "Leave request retrieved successfully.", await _leaveRequestService.GetByIdAsync(id)));

    [HttpPost]
    public async Task<ActionResult<ApiResponse<LeaveRequestResponseDto>>> Create(LeaveRequestCreateDto dto)
    {
        var leaveRequest = await _leaveRequestService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = leaveRequest.Id }, new ApiResponse<LeaveRequestResponseDto>(true, "Leave request created successfully.", leaveRequest));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<LeaveRequestResponseDto>>> Update(int id, LeaveRequestUpdateDto dto) => Ok(new ApiResponse<LeaveRequestResponseDto>(true, "Leave request updated successfully.", await _leaveRequestService.UpdateAsync(id, dto)));

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id) { await _leaveRequestService.DeleteAsync(id); return Ok(new ApiResponse<object>(true, "Leave request deleted successfully.", null)); }
}
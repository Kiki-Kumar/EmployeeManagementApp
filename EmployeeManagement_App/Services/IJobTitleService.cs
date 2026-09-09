using EmployeeManagementApp.DTOs;

namespace EmployeeManagementApp.Services;

public interface IJobTitleService
{
    Task<IReadOnlyList<JobTitleResponseDto>> GetAllAsync();
    Task<JobTitleResponseDto> GetByIdAsync(int id);
    Task<JobTitleResponseDto> CreateAsync(JobTitleCreateDto jobTitleCreateDto);
    Task<JobTitleResponseDto> UpdateAsync(int id, JobTitleUpdateDto jobTitleUpdateDto);
    Task DeleteAsync(int id);
}
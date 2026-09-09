using EmployeeManagementApp.DTOs;
using EmployeeManagementApp.Exceptions;
using EmployeeManagementApp.Models;
using EmployeeManagementApp.Repositories;

namespace EmployeeManagementApp.Services;

public class JobTitleService : IJobTitleService
{
    private readonly IJobTitleRepository _jobTitleRepository;
    private readonly ILogger<JobTitleService> _logger;
    public JobTitleService(IJobTitleRepository jobTitleRepository, ILogger<JobTitleService> logger) { _jobTitleRepository = jobTitleRepository; _logger = logger; }
    public async Task<IReadOnlyList<JobTitleResponseDto>> GetAllAsync() => (await _jobTitleRepository.GetAllAsync()).Select(MapToResponseDto).ToList();
    public async Task<JobTitleResponseDto> GetByIdAsync(int id) => MapToResponseDto(await FindJobTitleAsync(id));
    public async Task<JobTitleResponseDto> CreateAsync(JobTitleCreateDto dto)
    {
        await EnsureNameIsAvailableAsync(dto.Name);
        var jobTitle = await _jobTitleRepository.AddAsync(new JobTitle { Name = dto.Name, Description = dto.Description });
        _logger.LogInformation("Job title {JobTitleId} was created.", jobTitle.Id);
        return MapToResponseDto(jobTitle);
    }
    public async Task<JobTitleResponseDto> UpdateAsync(int id, JobTitleUpdateDto dto)
    {
        var jobTitle = await FindJobTitleAsync(id);
        await EnsureNameIsAvailableAsync(dto.Name, id);
        jobTitle.Name = dto.Name;
        jobTitle.Description = dto.Description;
        await _jobTitleRepository.UpdateAsync(jobTitle);
        return MapToResponseDto(jobTitle);
    }
    public async Task DeleteAsync(int id)
    {
        var jobTitle = await FindJobTitleAsync(id);
        if (await _jobTitleRepository.HasEmployeesAsync(id)) throw new ResourceConflictException("A job title with assigned employees cannot be deleted.");
        await _jobTitleRepository.DeleteAsync(jobTitle);
    }
    private async Task<JobTitle> FindJobTitleAsync(int id) => await _jobTitleRepository.GetByIdAsync(id) ?? throw new ResourceNotFoundException("Job title", id);
    private async Task EnsureNameIsAvailableAsync(string name, int? id = null) { if (await _jobTitleRepository.NameExistsAsync(name, id)) throw new ResourceConflictException("A job title with this name already exists."); }
    private static JobTitleResponseDto MapToResponseDto(JobTitle jobTitle) => new() { Id = jobTitle.Id, Name = jobTitle.Name, Description = jobTitle.Description };
}
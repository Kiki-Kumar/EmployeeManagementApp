using EmployeeManagementApp.Models;

namespace EmployeeManagementApp.Repositories;

public interface IJobTitleRepository
{
    Task<IReadOnlyList<JobTitle>> GetAllAsync();
    Task<JobTitle?> GetByIdAsync(int id);
    Task<bool> NameExistsAsync(string name, int? excludingId = null);
    Task<bool> HasEmployeesAsync(int id);
    Task<JobTitle> AddAsync(JobTitle jobTitle);
    Task UpdateAsync(JobTitle jobTitle);
    Task DeleteAsync(JobTitle jobTitle);
}
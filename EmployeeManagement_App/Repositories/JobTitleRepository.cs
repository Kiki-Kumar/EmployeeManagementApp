using EmployeeManagementApp.Data;
using EmployeeManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementApp.Repositories;

public class JobTitleRepository : IJobTitleRepository
{
    private readonly ApplicationDbContext _context;
    public JobTitleRepository(ApplicationDbContext context) => _context = context;
    public async Task<IReadOnlyList<JobTitle>> GetAllAsync() => await _context.JobTitles.AsNoTracking().OrderBy(jobTitle => jobTitle.Name).ToListAsync();
    public async Task<JobTitle?> GetByIdAsync(int id) => await _context.JobTitles.FirstOrDefaultAsync(jobTitle => jobTitle.Id == id);
    public async Task<bool> NameExistsAsync(string name, int? excludingId = null) => await _context.JobTitles.AnyAsync(jobTitle => jobTitle.Name == name && (!excludingId.HasValue || jobTitle.Id != excludingId));
    public async Task<bool> HasEmployeesAsync(int id) => await _context.Employees.AnyAsync(employee => employee.JobTitleId == id);
    public async Task<JobTitle> AddAsync(JobTitle jobTitle) { await _context.JobTitles.AddAsync(jobTitle); await _context.SaveChangesAsync(); return jobTitle; }
    public async Task UpdateAsync(JobTitle jobTitle) { await _context.SaveChangesAsync(); }
    public async Task DeleteAsync(JobTitle jobTitle) { _context.JobTitles.Remove(jobTitle); await _context.SaveChangesAsync(); }
}
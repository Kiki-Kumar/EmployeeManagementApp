using EmployeeManagementApp.Data;
using EmployeeManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementApp.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Employee>> GetAllAsync()
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(employee => employee.Department)
            .Include(employee => employee.JobTitle)
            .OrderBy(employee => employee.Id)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees
            .Include(employee => employee.Department)
            .Include(employee => employee.JobTitle)
            .FirstOrDefaultAsync(employee => employee.Id == id);
    }

    public async Task<bool> DepartmentExistsAsync(int departmentId)
    {
        return await _context.Departments.AnyAsync(department => department.Id == departmentId);
    }

    public async Task<bool> JobTitleExistsAsync(int jobTitleId)
    {
        return await _context.JobTitles.AnyAsync(jobTitle => jobTitle.Id == jobTitleId);
    }

    public async Task<Employee> AddAsync(Employee employee)
    {
        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task UpdateAsync(Employee employee)
    {
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Employee employee)
    {
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
    }
}
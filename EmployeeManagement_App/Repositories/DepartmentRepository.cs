using EmployeeManagementApp.Data;
using EmployeeManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementApp.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly ApplicationDbContext _context;
    public DepartmentRepository(ApplicationDbContext context) => _context = context;
    public async Task<IReadOnlyList<Department>> GetAllAsync() => await _context.Departments.AsNoTracking().OrderBy(department => department.Name).ToListAsync();
    public async Task<Department?> GetByIdAsync(int id) => await _context.Departments.FirstOrDefaultAsync(department => department.Id == id);
    public async Task<bool> NameExistsAsync(string name, int? excludingId = null) => await _context.Departments.AnyAsync(department => department.Name == name && (!excludingId.HasValue || department.Id != excludingId));
    public async Task<bool> HasEmployeesAsync(int id) => await _context.Employees.AnyAsync(employee => employee.DepartmentId == id);
    public async Task<Department> AddAsync(Department department) { await _context.Departments.AddAsync(department); await _context.SaveChangesAsync(); return department; }
    public async Task UpdateAsync(Department department) { await _context.SaveChangesAsync(); }
    public async Task DeleteAsync(Department department) { _context.Departments.Remove(department); await _context.SaveChangesAsync(); }
}
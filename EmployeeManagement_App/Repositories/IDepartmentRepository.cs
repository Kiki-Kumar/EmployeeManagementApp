using EmployeeManagementApp.Models;

namespace EmployeeManagementApp.Repositories;

public interface IDepartmentRepository
{
    Task<IReadOnlyList<Department>> GetAllAsync();
    Task<Department?> GetByIdAsync(int id);
    Task<bool> NameExistsAsync(string name, int? excludingId = null);
    Task<bool> HasEmployeesAsync(int id);
    Task<Department> AddAsync(Department department);
    Task UpdateAsync(Department department);
    Task DeleteAsync(Department department);
}
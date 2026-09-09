using EmployeeManagementApp.Models;

namespace EmployeeManagementApp.Repositories;

public interface IEmployeeRepository
{
    Task<IReadOnlyList<Employee>> GetAllAsync();

    Task<Employee?> GetByIdAsync(int id);

    Task<bool> DepartmentExistsAsync(int departmentId);

    Task<bool> JobTitleExistsAsync(int jobTitleId);

    Task<Employee> AddAsync(Employee employee);

    Task UpdateAsync(Employee employee);

    Task DeleteAsync(Employee employee);
}
using EmployeeManagementApp.DTOs;
using EmployeeManagementApp.Exceptions;
using EmployeeManagementApp.Models;
using EmployeeManagementApp.Repositories;

namespace EmployeeManagementApp.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<DepartmentService> _logger;
    public DepartmentService(IDepartmentRepository departmentRepository, ILogger<DepartmentService> logger) { _departmentRepository = departmentRepository; _logger = logger; }
    public async Task<IReadOnlyList<DepartmentResponseDto>> GetAllAsync() => (await _departmentRepository.GetAllAsync()).Select(MapToResponseDto).ToList();
    public async Task<DepartmentResponseDto> GetByIdAsync(int id) => MapToResponseDto(await FindDepartmentAsync(id));
    public async Task<DepartmentResponseDto> CreateAsync(DepartmentCreateDto dto)
    {
        await EnsureNameIsAvailableAsync(dto.Name);
        var department = await _departmentRepository.AddAsync(new Department { Name = dto.Name, Description = dto.Description });
        _logger.LogInformation("Department {DepartmentId} was created.", department.Id);
        return MapToResponseDto(department);
    }
    public async Task<DepartmentResponseDto> UpdateAsync(int id, DepartmentUpdateDto dto)
    {
        var department = await FindDepartmentAsync(id);
        await EnsureNameIsAvailableAsync(dto.Name, id);
        department.Name = dto.Name;
        department.Description = dto.Description;
        await _departmentRepository.UpdateAsync(department);
        return MapToResponseDto(department);
    }
    public async Task DeleteAsync(int id)
    {
        var department = await FindDepartmentAsync(id);
        if (await _departmentRepository.HasEmployeesAsync(id)) throw new ResourceConflictException("A department with assigned employees cannot be deleted.");
        await _departmentRepository.DeleteAsync(department);
    }
    private async Task<Department> FindDepartmentAsync(int id) => await _departmentRepository.GetByIdAsync(id) ?? throw new ResourceNotFoundException("Department", id);
    private async Task EnsureNameIsAvailableAsync(string name, int? id = null) { if (await _departmentRepository.NameExistsAsync(name, id)) throw new ResourceConflictException("A department with this name already exists."); }
    private static DepartmentResponseDto MapToResponseDto(Department department) => new() { Id = department.Id, Name = department.Name, Description = department.Description };
}
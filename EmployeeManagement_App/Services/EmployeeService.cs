using EmployeeManagementApp.DTOs;
using EmployeeManagementApp.Exceptions;
using EmployeeManagementApp.Models;
using EmployeeManagementApp.Repositories;
using EmployeeManagement.Contracts.Events;
namespace EmployeeManagementApp.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<EmployeeService> _logger;
    private readonly IEmployeeEventPublisher _employeeEventPublisher;

    public EmployeeService(IEmployeeRepository employeeRepository, ILogger<EmployeeService> logger, IEmployeeEventPublisher employeeEventPublisher)
    {
        _employeeRepository = employeeRepository;
        _logger = logger;
        _employeeEventPublisher = employeeEventPublisher;
    }

    public async Task<IReadOnlyList<EmployeeResponseDto>> GetAllAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();
        return employees.Select(MapToResponseDto).ToList();
    }

    public async Task<EmployeeResponseDto> GetByIdAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee is null)
        {
            throw new EmployeeNotFoundException(id);
        }

        return MapToResponseDto(employee);
    }

    public async Task<EmployeeResponseDto> CreateAsync(
     EmployeeCreateDto employeeCreateDto)
    {
        _logger.LogInformation(
            "Employee creation started for {Email}.",
            employeeCreateDto.Email);

        await ValidateEmployeeReferencesAsync(
            employeeCreateDto.DepartmentId,
            employeeCreateDto.JobTitleId);

        var employee = new Employee
        {
            FirstName = employeeCreateDto.FirstName,
            LastName = employeeCreateDto.LastName,
            Email = employeeCreateDto.Email,
            DepartmentId = employeeCreateDto.DepartmentId,
            JobTitleId = employeeCreateDto.JobTitleId,
            Salary = employeeCreateDto.Salary,
            DateOfJoining = employeeCreateDto.DateOfJoining,
            IsActive = employeeCreateDto.IsActive
        };

        // Save employee to database
        var createdEmployee = await _employeeRepository.AddAsync(employee);

        _logger.LogInformation(
            "Employee {EmployeeId} was created successfully.",
            createdEmployee.Id);

        // Create employee-created event
        var employeeCreatedEvent = new EmployeeCreatedEvent
        {
            EmployeeId = createdEmployee.Id,
            Name = $"{createdEmployee.FirstName} {createdEmployee.LastName}",
            Email = createdEmployee.Email,
            Department = createdEmployee.DepartmentId.ToString()
        };

        // Publish event to Azure Storage Queue
        await _employeeEventPublisher.PublishEmployeeCreatedAsync(
            employeeCreatedEvent);

        // Get employee with related data
        var employeeWithReferences =
            await _employeeRepository.GetByIdAsync(createdEmployee.Id)
            ?? throw new EmployeeNotFoundException(createdEmployee.Id);

        return MapToResponseDto(employeeWithReferences);
    }

    public async Task<EmployeeResponseDto> UpdateAsync(int id, EmployeeUpdateDto employeeUpdateDto)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee is null)
        {
            throw new EmployeeNotFoundException(id);
        }

        await ValidateEmployeeReferencesAsync(employeeUpdateDto.DepartmentId, employeeUpdateDto.JobTitleId);

        employee.FirstName = employeeUpdateDto.FirstName;
        employee.LastName = employeeUpdateDto.LastName;
        employee.Email = employeeUpdateDto.Email;
        employee.DepartmentId = employeeUpdateDto.DepartmentId;
        employee.JobTitleId = employeeUpdateDto.JobTitleId;
        employee.Salary = employeeUpdateDto.Salary;
        employee.DateOfJoining = employeeUpdateDto.DateOfJoining;
        employee.IsActive = employeeUpdateDto.IsActive;

        await _employeeRepository.UpdateAsync(employee);
        _logger.LogInformation("Employee {EmployeeId} was updated successfully.", id);
        return MapToResponseDto(employee);
    }

    public async Task DeleteAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee is null)
        {
            throw new EmployeeNotFoundException(id);
        }

        await _employeeRepository.DeleteAsync(employee);
        _logger.LogInformation("Employee {EmployeeId} was deleted successfully.", id);
    }

    private static EmployeeResponseDto MapToResponseDto(Employee employee)
    {
        return new EmployeeResponseDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department.Name,
            JobTitleId = employee.JobTitleId,
            JobTitleName = employee.JobTitle.Name,
            Salary = employee.Salary,
            DateOfJoining = employee.DateOfJoining,
            IsActive = employee.IsActive
        };
    }

    private async Task ValidateEmployeeReferencesAsync(int departmentId, int jobTitleId)
    {
        if (!await _employeeRepository.DepartmentExistsAsync(departmentId))
        {
            throw new InvalidEmployeeReferenceException("The selected department does not exist.");
        }

        if (!await _employeeRepository.JobTitleExistsAsync(jobTitleId))
        {
            throw new InvalidEmployeeReferenceException("The selected job title does not exist.");
        }
    }
}
using EmployeeManagement.Contracts.Events;

namespace EmployeeManagementApp.Services
{
    public interface IEmployeeEventPublisher
    {
        Task PublishEmployeeCreatedAsync(EmployeeCreatedEvent employeeCreatedEvent);
    }
}
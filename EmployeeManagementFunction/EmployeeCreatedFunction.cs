using System.Text.Json;
using EmployeeManagementFunction.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using EmployeeManagement.Contracts.Events;
namespace EmployeeManagementFunction
{
    public class EmployeeCreatedFunction
    {
        private readonly ILogger<EmployeeCreatedFunction> _logger;
        private readonly IEmailService _emailService;

        public EmployeeCreatedFunction(
            ILogger<EmployeeCreatedFunction> logger,
            IEmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
        }

        [Function("EmployeeCreatedFunction")]
        public async Task Run(
            [QueueTrigger(
                "employee-created",
                Connection = "AzureWebJobsStorage")]
            string message)
        {
            _logger.LogInformation(
                "Employee-created message received from Azure Queue.");

            _logger.LogInformation(
                "Message: {Message}",
                message);

            try
            {
                var employeeCreatedEvent =
                    JsonSerializer.Deserialize<EmployeeCreatedEvent>(message);

                if (employeeCreatedEvent == null)
                {
                    throw new InvalidOperationException(
                        "Unable to deserialize employee-created event.");
                }

                _logger.LogInformation(
                    "Processing welcome email for EmployeeId {EmployeeId}.",
                    employeeCreatedEvent.EmployeeId);

                await _emailService.SendWelcomeEmailAsync(
                    employeeCreatedEvent.Email,
                    employeeCreatedEvent.Name);

                _logger.LogInformation(
                    "Welcome email processing completed for EmployeeId {EmployeeId}.",
                    employeeCreatedEvent.EmployeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to process employee-created message.");

                throw;
            }
        }
    }
}
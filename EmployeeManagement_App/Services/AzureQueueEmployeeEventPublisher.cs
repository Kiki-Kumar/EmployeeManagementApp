using System.Text.Json;
using Azure;
using Azure.Storage.Queues;
using EmployeeManagement.Contracts.Events;

namespace EmployeeManagementApp.Services
{
    public class AzureQueueEmployeeEventPublisher : IEmployeeEventPublisher
    {
        private readonly QueueClient _queueClient;
        private readonly ILogger<AzureQueueEmployeeEventPublisher> _logger;

        public AzureQueueEmployeeEventPublisher(
            IConfiguration configuration,
            ILogger<AzureQueueEmployeeEventPublisher> logger)
        {
            _logger = logger;

            // Get Azure Storage connection string from User Secrets
            var connectionString =
                configuration["AzureStorage:ConnectionString"];

            // Get queue name from configuration
            var queueName =
                configuration["AzureStorage:QueueName"];

            // Validate connection string
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Azure Storage connection string is not configured.");
            }

            // Validate queue name
            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new InvalidOperationException(
                    "Azure Storage Queue name is not configured.");
            }

            // Configure message encoding
            var queueOptions = new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };

            // Create Azure Queue client
            _queueClient = new QueueClient(
                connectionString,
                queueName,
                queueOptions);

            _logger.LogInformation(
                "Azure Queue client created successfully for queue {QueueName}.",
                queueName);
        }

        public async Task PublishEmployeeCreatedAsync(
            EmployeeCreatedEvent employeeCreatedEvent)
        {
            try
            {
                // Convert event object into JSON
                var message =
                    JsonSerializer.Serialize(employeeCreatedEvent);

                _logger.LogInformation(
                    "Publishing employee-created event for EmployeeId {EmployeeId}.",
                    employeeCreatedEvent.EmployeeId);

                // Send Base64-encoded message to Azure Storage Queue
                await _queueClient.SendMessageAsync(message);

                _logger.LogInformation(
                    "Employee-created event published successfully for EmployeeId {EmployeeId}.",
                    employeeCreatedEvent.EmployeeId);
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(
                    ex,
                    "Azure Storage Queue request failed. " +
                    "StatusCode: {StatusCode}, ErrorCode: {ErrorCode}",
                    ex.Status,
                    ex.ErrorCode);

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error while publishing employee-created event.");

                throw;
            }
        }
    }
}
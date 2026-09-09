namespace EmployeeManagementFunction.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(
            string recipientEmail,
            string recipientName);
    }
}
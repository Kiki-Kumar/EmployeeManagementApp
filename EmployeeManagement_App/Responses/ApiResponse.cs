namespace EmployeeManagementApp.Responses;

public class ApiResponse<T>
{
    public ApiResponse(bool success, string message, T? data)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    public bool Success { get; }

    public string Message { get; }

    public T? Data { get; }
}
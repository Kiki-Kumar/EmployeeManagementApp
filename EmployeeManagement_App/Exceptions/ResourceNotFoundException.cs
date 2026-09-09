namespace EmployeeManagementApp.Exceptions;

public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException(string resourceName, int resourceId)
        : base($"{resourceName} with ID {resourceId} was not found.")
    {
        ResourceName = resourceName;
        ResourceId = resourceId;
    }

    public string ResourceName { get; }

    public int ResourceId { get; }
}
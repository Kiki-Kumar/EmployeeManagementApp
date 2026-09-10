using System.Net.Http.Json;
using EmployeeManagementApp.DTOs;
using EmployeeManagementApp.Exceptions;
using EmployeeManagementApp.Responses;

namespace EmployeeManagementApp.Services;

public class ProjectsApiClient : IProjectsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProjectsApiClient> _logger;

    public ProjectsApiClient(HttpClient httpClient, ILogger<ProjectsApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ProjectResponseDto>> GetProjectsByEmployeeIdAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Requesting projects for employee {EmployeeId} from {ProjectsApiUrl}.", employeeId, _httpClient.BaseAddress);

            var response = await _httpClient.GetAsync($"api/projects/employee/{employeeId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Projects API returned status {StatusCode} for employee {EmployeeId}.",
                    response.StatusCode,
                    employeeId);

                throw new ProjectsApiUnavailableException("The Projects API is unavailable or returned an unexpected status.");
            }

            var payload = await response.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyList<ProjectResponseDto>>>(cancellationToken: cancellationToken);

            if (payload?.Data is null)
            {
                return Array.Empty<ProjectResponseDto>();
            }

            return payload.Data;
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "Projects API request failed for employee {EmployeeId}.", employeeId);
            throw new ProjectsApiUnavailableException("The Projects API is currently unavailable.");
        }
        catch (TaskCanceledException exception)
        {
            _logger.LogError(exception, "Projects API request timed out for employee {EmployeeId}.", employeeId);
            throw new ProjectsApiUnavailableException("The Projects API request timed out.");
        }
    }
}

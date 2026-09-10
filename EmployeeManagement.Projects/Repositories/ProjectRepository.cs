using EmployeeManagement.Projects.Data;
using EmployeeManagement.Projects.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Projects.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ProjectsDbContext _context;

    public ProjectRepository(ProjectsDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Project>> GetAllAsync()
    {
        return await _context.Projects
            .AsNoTracking()
            .OrderBy(project => project.ProjectId)
            .ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(int projectId)
    {
        return await _context.Projects
            .FirstOrDefaultAsync(project => project.ProjectId == projectId);
    }

    public async Task<IReadOnlyList<Project>> GetByEmployeeIdAsync(int employeeId)
    {
        return await _context.Projects
            .AsNoTracking()
            .Where(project => project.EmployeeId == employeeId)
            .OrderBy(project => project.ProjectId)
            .ToListAsync();
    }

    public async Task<Project> AddAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();
        return project;
    }

    public async Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Project project)
    {
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
    }
}

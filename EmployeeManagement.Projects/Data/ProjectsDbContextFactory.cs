using EmployeeManagement.Projects.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EmployeeManagement.Projects.Data;

public class ProjectsDbContextFactory : IDesignTimeDbContextFactory<ProjectsDbContext>
{
    public ProjectsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ProjectsDbContext>();
        optionsBuilder.UseSqlServer("Server=KANINI-LTP-579;Database=EmployeeManagementProjects;Integrated Security=SSPI;TrustServerCertificate=True;");

        return new ProjectsDbContext(optionsBuilder.Options);
    }
}

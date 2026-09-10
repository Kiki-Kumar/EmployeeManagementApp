using EmployeeManagement.Projects.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Projects.Data;

public class ProjectsDbContext : DbContext
{
    public ProjectsDbContext(DbContextOptions<ProjectsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(project => project.ProjectId);
            entity.Property(project => project.ProjectName)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(project => project.Description)
                .HasMaxLength(1000);

            entity.Property(project => project.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(project => project.EmployeeId)
                .IsRequired();

            entity.HasIndex(project => project.EmployeeId);
        });
    }
}

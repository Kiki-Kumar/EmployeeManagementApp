using EmployeeManagementApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }

    public DbSet<Department> Departments { get; set; }

    public DbSet<JobTitle> JobTitles { get; set; }

    public DbSet<LeaveRequest> LeaveRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(employee => employee.FirstName).HasMaxLength(100);
            entity.Property(employee => employee.LastName).HasMaxLength(100);
            entity.Property(employee => employee.Email).HasMaxLength(256);
            entity.Property(employee => employee.Salary).HasPrecision(18, 2);

            entity.HasOne(employee => employee.Department)
                .WithMany(department => department.Employees)
                .HasForeignKey(employee => employee.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(employee => employee.JobTitle)
                .WithMany(jobTitle => jobTitle.Employees)
                .HasForeignKey(employee => employee.JobTitleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.Property(department => department.Name).HasMaxLength(100);
            entity.Property(department => department.Description).HasMaxLength(500);
            entity.HasIndex(department => department.Name).IsUnique();
        });

        modelBuilder.Entity<JobTitle>(entity =>
        {
            entity.Property(jobTitle => jobTitle.Name).HasMaxLength(100);
            entity.Property(jobTitle => jobTitle.Description).HasMaxLength(500);
            entity.HasIndex(jobTitle => jobTitle.Name).IsUnique();
        });

        modelBuilder.Entity<LeaveRequest>(entity => 
        {
            entity.Property(request => request.Reason).HasMaxLength(500);
            entity.Property(request => request.Status).HasConversion<string>().HasMaxLength(20);

            entity.HasOne(request => request.Employee)
                .WithMany(employee => employee.LeaveRequests)
                .HasForeignKey(request => request.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
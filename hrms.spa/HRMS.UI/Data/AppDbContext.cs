using HRMS.UI.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace HRMS.UI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Occupation> Occupations => Set<Occupation>();
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Occupation)
            .WithMany(o => o.Employees)
            .HasForeignKey(e => e.OccupationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

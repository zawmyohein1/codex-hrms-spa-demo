using HRMS.UI.Models.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace HRMS.UI.DataAccess;

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

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(200);
            entity.HasIndex(d => d.Name)
                .IsUnique();
        });

        modelBuilder.Entity<Occupation>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(200);
            entity.HasIndex(o => o.Name)
                .IsUnique();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmpNo)
                .IsRequired()
                .HasMaxLength(50);
            entity.HasIndex(e => e.EmpNo)
                .IsUnique();

            entity.Property(e => e.EmpName)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.JobTitle)
                .HasMaxLength(100);
            entity.Property(e => e.Gender)
                .HasMaxLength(10);
            entity.Property(e => e.HiredDate)
                .HasColumnType("date");
            entity.Property(e => e.ResignDate)
                .HasColumnType("date");
            entity.Property(e => e.DateOfBirth)
                .HasColumnType("date");
            entity.Property(e => e.Phone)
                .HasMaxLength(50);
            entity.Property(e => e.Email)
                .HasMaxLength(256);
            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasFilter("[Email] IS NOT NULL");
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(500);

            entity.HasOne(e => e.Department)
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Occupation)
                .WithMany()
                .HasForeignKey(e => e.OccupationId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

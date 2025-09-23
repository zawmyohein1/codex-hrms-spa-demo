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
    }
}

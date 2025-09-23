using HRMS.UI.Models.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace HRMS.UI.DataAccess;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Occupation> Occupations => Set<Occupation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Departments");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name)
                .HasMaxLength(200)
                .IsRequired();
            entity.HasIndex(d => d.Name).IsUnique();
        });

        modelBuilder.Entity<Occupation>(entity =>
        {
            entity.ToTable("Occupations");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Name)
                .HasMaxLength(200)
                .IsRequired();
            entity.HasIndex(o => o.Name).IsUnique();
        });
    }
}

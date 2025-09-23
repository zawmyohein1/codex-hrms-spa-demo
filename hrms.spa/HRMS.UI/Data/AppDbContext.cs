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
}

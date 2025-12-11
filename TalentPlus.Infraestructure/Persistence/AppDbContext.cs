using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TalentPlus.Domain.Entities;
using TalentPlus.Infraestructure.Identity;

namespace TalentPlus.Infraestructure.Persistence;

// Inherits from IdentityDbContext to handle Users AND Business Tables
public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Constraint: Document Number must be unique
        builder.Entity<Employee>()
            .HasIndex(e => e.DocumentNumber)
            .IsUnique();

        // Constraint: Postgres needs explicit precision for decimals
        builder.Entity<Employee>()
            .Property(e => e.Salary)
            .HasColumnType("decimal(18,2)");
    }
}
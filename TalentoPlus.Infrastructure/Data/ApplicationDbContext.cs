using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;

using TalentoPlus.Application.Common.Interfaces;

namespace TalentoPlus.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<Person>(options), IApplicationDbContext
{
    public DbSet<Worker> Workers { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Sale> Sales { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Workers y Admins usan la tabla AspNetUsers (TPH - Table Per Hierarchy)
        // No necesitan configuración de tabla separada
        
        // Configure Sale relationship
        builder.Entity<Sale>()
            .HasOne(s => s.Worker)
            .WithMany(w => w.Sales)
            .HasForeignKey(s => s.WorkerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

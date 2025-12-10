using Microsoft.EntityFrameworkCore;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Worker> Workers { get; }
    DbSet<Admin> Admins { get; }
    DbSet<Sale> Sales { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

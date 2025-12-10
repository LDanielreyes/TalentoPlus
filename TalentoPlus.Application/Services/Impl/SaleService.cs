using Microsoft.EntityFrameworkCore;
using TalentoPlus.Application.Common.Interfaces;
using TalentoPlus.Application.Services;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Application.Services.Impl;

public class SaleService(IApplicationDbContext context) : ISaleService
{
    public async Task<IEnumerable<Sale>> GetAllSalesAsync()
    {
        return await context.Sales.Include(s => s.Worker).ToListAsync();
    }

    public async Task<Sale?> GetSaleByIdAsync(int id)
    {
        return await context.Sales.Include(s => s.Worker).FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task CreateSaleAsync(Sale sale)
    {
        context.Sales.Add(sale);
        await context.SaveChangesAsync(CancellationToken.None);
    }

    public async Task UpdateSaleAsync(Sale sale)
    {
        context.Sales.Update(sale);
        await context.SaveChangesAsync(CancellationToken.None);
    }

    public async Task DeleteSaleAsync(int id)
    {
        var sale = await context.Sales.FindAsync(id);
        if (sale != null)
        {
            context.Sales.Remove(sale);
            await context.SaveChangesAsync(CancellationToken.None);
        }
    }

    public async Task<IEnumerable<Sale>> GetSalesByWorkerIdAsync(string workerId)
    {
        return await context.Sales
            .Where(s => s.WorkerId == workerId)
            .Include(s => s.Worker)
            .ToListAsync();
    }
}

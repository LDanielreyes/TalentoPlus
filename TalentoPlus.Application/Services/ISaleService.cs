using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Application.Services;

public interface ISaleService
{
    Task<IEnumerable<Sale>> GetAllSalesAsync();
    Task<Sale?> GetSaleByIdAsync(int id);
    Task CreateSaleAsync(Sale sale);
    Task UpdateSaleAsync(Sale sale);
    Task DeleteSaleAsync(int id);
    Task<IEnumerable<Sale>> GetSalesByWorkerIdAsync(string workerId);
}

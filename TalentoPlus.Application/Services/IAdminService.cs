using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Application.Services;

public interface IAdminService
{
    Task<Admin?> GetAdminByIdAsync(string id);
    Task<int> GetTotalWorkersAsync();
    Task<decimal> GetTotalSalesAmountAsync();
    Task<int> GetWorkersOnVacationAsync();
    Task<int> GetActiveWorkersAsync();
    Task<string> ProcessAIQueryAsync(string query);
}

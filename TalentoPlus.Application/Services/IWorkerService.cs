using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Application.Services;

public interface IWorkerService
{
    Task<IEnumerable<Worker>> GetAllWorkersAsync();
    Task<(IEnumerable<Worker> Workers, int TotalCount)> GetWorkersAsync(string? searchString, int pageNumber, int pageSize);
    Task<Worker?> GetWorkerByIdAsync(string id);
    Task CreateWorkerAsync(Worker worker);
    Task UpdateWorkerAsync(Worker worker);
    Task DeleteWorkerAsync(string id);
    Task ImportWorkersFromExcelAsync(Stream fileStream);
}

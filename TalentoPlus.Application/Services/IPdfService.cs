using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Application.Services;

public interface IPdfService
{
    byte[] GenerateWorkerCv(Worker worker);
}

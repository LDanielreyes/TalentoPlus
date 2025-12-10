using Microsoft.EntityFrameworkCore;
using TalentoPlus.Application.Common.Interfaces;
using TalentoPlus.Application.Services;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Application.Services.Impl;

public class AdminService(IApplicationDbContext context, IAIService aiService) : IAdminService
{
    public async Task<Admin?> GetAdminByIdAsync(string id)
    {
        return await context.Admins.FindAsync(id);
    }

    public async Task<int> GetTotalWorkersAsync()
    {
        return await context.Workers.CountAsync();
    }

    public async Task<decimal> GetTotalSalesAmountAsync()
    {
        return await context.Sales.SumAsync(s => s.Amount);
    }

    public async Task<int> GetWorkersOnVacationAsync()
    {
        return await context.Workers.CountAsync(w => w.Status == WorkerStatus.Vacaciones);
    }

    public async Task<int> GetActiveWorkersAsync()
    {
        return await context.Workers.CountAsync(w => w.Status == WorkerStatus.Activo);
    }

    public async Task<string> ProcessAIQueryAsync(string query)
    {
        // Ask AI to interpret the query
        var aiResponse = await aiService.ProcessQueryAsync(query);
        
        // Execute the query based on AI interpretation
        var result = await ExecuteQuerySafelyAsync(aiResponse);
        
        return result;
    }

    private async Task<string> ExecuteQuerySafelyAsync(AIQueryResponse aiResponse)
    {
        try
        {
            // Based on AI's interpretation, execute predefined safe queries
            switch (aiResponse.QueryType.ToLower())
            {
                case "count_workers":
                    var count = await context.Workers.CountAsync();
                    return $"Total de empleados: {count}";

                case "count_by_status":
                    if (Enum.TryParse<WorkerStatus>(aiResponse.Parameter, true, out var status))
                    {
                        var statusCount = await context.Workers.CountAsync(w => w.Status == status);
                        return $"Empleados con estado '{aiResponse.Parameter}': {statusCount}";
                    }
                    return "Estado no encontrado";

                case "count_by_department":
                    if (Enum.TryParse<Department>(aiResponse.Parameter, true, out var department))
                    {
                        var deptCount = await context.Workers.CountAsync(w => w.Department == department);
                        return $"Empleados en departamento '{aiResponse.Parameter}': {deptCount}";
                    }
                    return "Departamento no encontrado";

                case "count_by_position":
                    var positionCount = await context.Workers.CountAsync(w => w.Position.ToLower().Contains(aiResponse.Parameter.ToLower()));
                    return $"Empleados con cargo '{aiResponse.Parameter}': {positionCount}";

                case "count_by_education":
                    if (Enum.TryParse<EducationalLevel>(aiResponse.Parameter, true, out var education))
                    {
                        var eduCount = await context.Workers.CountAsync(w => w.EducationalLevel == education);
                        return $"Empleados con nivel educativo '{aiResponse.Parameter}': {eduCount}";
                    }
                    return "Nivel educativo no encontrado";

                default:
                    return "No pude entender tu consulta. Por favor, intenta reformularla.";
            }
        }
        catch (Exception ex)
        {
            return $"Error al procesar consulta: {ex.Message}";
        }
    }
}

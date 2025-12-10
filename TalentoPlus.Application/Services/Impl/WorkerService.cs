using Microsoft.EntityFrameworkCore;
using TalentoPlus.Application.Common.Interfaces;
using TalentoPlus.Application.Services;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

namespace TalentoPlus.Application.Services.Impl;

public class WorkerService(
    IApplicationDbContext context, 
    ILogger<WorkerService> logger,
    UserManager<Person> userManager) : IWorkerService
{
    public async Task<IEnumerable<Worker>> GetAllWorkersAsync()
    {
        return await context.Workers.ToListAsync();
    }

    public async Task<(IEnumerable<Worker> Workers, int TotalCount)> GetWorkersAsync(string? searchString, int pageNumber, int pageSize)
    {
        var query = context.Workers.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            searchString = searchString.ToLower();
            query = query.Where(w => w.FullName.ToLower().Contains(searchString) || 
                                     w.Email.ToLower().Contains(searchString));
        }

        var totalCount = await query.CountAsync();
        var workers = await query.Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();

        return (workers, totalCount);
    }

    public async Task<Worker?> GetWorkerByIdAsync(string id)
    {
        return await context.Workers.FindAsync(id);
    }

    public async Task CreateWorkerAsync(Worker worker)
    {
        // Set username to email if not provided
        if (string.IsNullOrWhiteSpace(worker.UserName))
        {
            worker.UserName = worker.Email;
        }
        
        // Set register date if not set
        if (worker.RegisterDate == default)
        {
            worker.RegisterDate = DateTime.UtcNow;
        }
        
        // Auto-confirm email
        worker.EmailConfirmed = true;
        
        // Create worker with default password
        var defaultPassword = "Worker@123";
        var result = await userManager.CreateAsync(worker, defaultPassword);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create worker: {errors}");
        }
        
        // Assign Worker role
        var roleResult = await userManager.AddToRoleAsync(worker, "Worker");
        if (!roleResult.Succeeded)
        {
            var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
            logger.LogWarning($"Failed to assign Worker role to '{worker.Email}': {errors}");
        }
        
        logger.LogInformation($"Worker '{worker.FullName}' ({worker.Email}) created successfully with Worker role");
    }

    public async Task UpdateWorkerAsync(Worker worker)
    {
        var result = await userManager.UpdateAsync(worker);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to update worker: {errors}");
        }
        
        logger.LogInformation($"Worker '{worker.FullName}' ({worker.Email}) updated successfully");
    }

    public async Task DeleteWorkerAsync(string id)
    {
        var worker = await userManager.FindByIdAsync(id);
        if (worker != null)
        {
            var result = await userManager.DeleteAsync(worker);
            
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to delete worker: {errors}");
            }
            
            logger.LogInformation($"Worker '{worker.FullName}' ({worker.Email}) deleted successfully");
        }
    }

    public async Task ImportWorkersFromExcelAsync(Stream fileStream)
    {
        using var workbook = new ClosedXML.Excel.XLWorkbook(fileStream);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row

        int successCount = 0;
        int errorCount = 0;
        int skippedCount = 0;

        logger.LogInformation("Starting Excel Import. Processing rows...");

        foreach (var row in rows)
        {
            try 
            {
                // Mapeo según las columnas del Excel:
                // 1=Nombres, 2=Apellidos, 3=FechaNacimiento, 4=Direccion, 5=Telefono, 
                // 6=Email, 7=Cargo, 8=Salario, 9=FechaIngreso, 10=Estado, 
                // 11=NivelEducativo, 12=PerfilProfesional, 13=DocumentoIdentidad, 14=Departamento
                
                // Helper para obtener valores de forma segura
                string GetCellString(int colNum) => row.Cell(colNum).IsEmpty() ? "" : row.Cell(colNum).GetString().Trim();
                
                int GetCellInt(int colNum)
                {
                    var cell = row.Cell(colNum);
                    if (cell.IsEmpty()) return 0;
                    
                    // Intentar obtener como número directamente
                    if (cell.TryGetValue<int>(out int intValue))
                        return intValue;
                    
                    // Intentar parsear el string
                    var strValue = cell.GetString().Trim();
                    if (int.TryParse(strValue, out int parsed))
                        return parsed;
                    
                    // Si falla, retornar 0
                    logger.LogWarning($"Row {row.RowNumber()}, Col {colNum}: Cannot parse '{strValue}' as int, using 0");
                    return 0;
                }
                
                var documentoIdentidad = GetCellString(1);
                var firstName = GetCellString(2);
                var lastName = GetCellString(3);
                var birthDate = GetCellString(4);
                var address = GetCellString(5);
                var phone = GetCellString(6);
                var email = GetCellString(7);
                var position = GetCellString(8);
                var wage = GetCellInt(9);
                var entryDate = GetCellString(10);
                var statusStr = GetCellString(11);
                var educationStr = GetCellString(12);
                var professionalProfile = GetCellString(13);
                var departmentStr = GetCellString(14);

                var fullName = $"{firstName} {lastName}".Trim();

                // Validar que al menos tenga nombre o email
                if (string.IsNullOrWhiteSpace(fullName) && string.IsNullOrWhiteSpace(email))
                {
                    logger.LogWarning($"Row {row.RowNumber()}: Skipping empty row (no name or email)");
                    skippedCount++;
                    continue;
                }

                // Validar email obligatorio
                if (string.IsNullOrWhiteSpace(email))
                {
                    logger.LogWarning($"Row {row.RowNumber()}: Skipping row - email is required");
                    skippedCount++;
                    continue;
                }

                // Verificar si el email ya existe
                var existingUser = await userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    logger.LogWarning($"Row {row.RowNumber()}: Email '{email}' already exists, skipping");
                    skippedCount++;
                    continue;
                }

                // Parsear enums
                if (!Enum.TryParse<WorkerStatus>(statusStr, true, out var status))
                {
                    logger.LogWarning($"Row {row.RowNumber()}: Invalid status '{statusStr}', using Activo");
                    status = WorkerStatus.Activo;
                }

                if (!Enum.TryParse<EducationalLevel>(educationStr, true, out var educationalLevel))
                {
                    logger.LogWarning($"Row {row.RowNumber()}: Invalid educational level '{educationStr}', using Tecnico");
                    educationalLevel = EducationalLevel.Tecnico;
                }

                if (!Enum.TryParse<Department>(departmentStr, true, out var department))
                {
                    logger.LogWarning($"Row {row.RowNumber()}: Invalid department '{departmentStr}', using Tecnología");
                    department = Department.Tecnología;
                }

                // Crear el Worker
                var worker = new Worker
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    DocumentoIdentidad = documentoIdentidad,
                    Address = address,
                    PhoneNumber = phone,
                    Position = position,
                    Wage = wage,
                    Status = status,
                    EducationalLevel = educationalLevel,
                    Department = department,
                    ProfessionalProfile = professionalProfile,
                    RegisterDate = DateTime.UtcNow,
                    EmailConfirmed = true // Auto-confirmar emails importados
                };

                // Crear usuario con contraseña por defecto
                var defaultPassword = "Worker@123"; // Se debe cambiar en el primer login
                var result = await userManager.CreateAsync(worker, defaultPassword);

                if (result.Succeeded)
                {
                    // Assign Worker role
                    var roleResult = await userManager.AddToRoleAsync(worker, "Worker");
                    if (!roleResult.Succeeded)
                    {
                        var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        logger.LogWarning($"Row {row.RowNumber()}: Failed to assign Worker role to '{fullName}': {roleErrors}");
                    }
                    
                    successCount++;
                    logger.LogInformation($"Row {row.RowNumber()}: Worker '{fullName}' ({email}) imported successfully with Worker role");
                }
                else
                {
                    errorCount++;
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    logger.LogError($"Row {row.RowNumber()}: Failed to create worker '{fullName}' - {errors}");
                }
            }
            catch (Exception ex)
            {
                errorCount++;
                logger.LogError($"Row {row.RowNumber()}: Error processing row - {ex.Message}");
            }
        }

        logger.LogInformation($"Excel Import completed. Success: {successCount}, Errors: {errorCount}, Skipped: {skippedCount}");
    }
}

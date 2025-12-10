using Xunit;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Infrastructure.Data;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Test.Integration;

public class DatabaseIntegrationTests
{
    [Fact]
    public async Task Database_CanConnectAndQueryWorkers()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase_Workers")
            .Options;

        // Act & Assert
        using (var context = new ApplicationDbContext(options))
        {
            // Add test data
            var worker = new Worker
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "test@test.com",
                Email = "test@test.com",
                FullName = "Test Worker",
                Position = "Developer",
                Wage = 5000,
                Department = Department.Tecnología,
                Status = WorkerStatus.Activo,
                EducationalLevel = EducationalLevel.Profesional,
                DocumentoIdentidad = "123456789"
            };

            context.Workers.Add(worker);
            await context.SaveChangesAsync();

            // Verify
            var savedWorker = await context.Workers.FirstOrDefaultAsync(w => w.Email == "test@test.com");
            Assert.NotNull(savedWorker);
            Assert.Equal("Test Worker", savedWorker.FullName);
            Assert.Equal(5000, savedWorker.Wage);
        }
    }
}

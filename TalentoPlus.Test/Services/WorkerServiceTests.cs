using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Application.Services.Impl;
using TalentoPlus.Application.Common.Interfaces;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Test.Services;

public class WorkerServiceTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly Mock<ILogger<WorkerService>> _mockLogger;
    private readonly Mock<UserManager<Person>> _mockUserManager;
    private readonly WorkerService _service;

    public WorkerServiceTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockLogger = new Mock<ILogger<WorkerService>>();
        
        // Mock UserManager (required setup)
        var store = new Mock<IUserStore<Person>>();
        _mockUserManager = new Mock<UserManager<Person>>(
            store.Object, null, null, null, null, null, null, null, null);

        _service = new WorkerService(
            _mockContext.Object,
            _mockLogger.Object,
            _mockUserManager.Object);
    }

    [Fact]
    public async Task GetWorkerByIdAsync_ExistingId_ReturnsWorker()
    {
        // Arrange
        var workerId = "test-id-123";
        var expectedWorker = new Worker
        {
            Id = workerId,
            FullName = "Test Worker",
            Email = "test@test.com",
            Position = "Tester",
            Wage = 4000,
            Department = Department.Tecnología,
            Status = WorkerStatus.Activo,
            EducationalLevel = EducationalLevel.Tecnico
        };

        _mockContext.Setup(c => c.Workers.FindAsync(workerId))
            .ReturnsAsync(expectedWorker);

        // Act
        var result = await _service.GetWorkerByIdAsync(workerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(workerId, result.Id);
        Assert.Equal("Test Worker", result.FullName);
        Assert.Equal("test@test.com", result.Email);
    }
}

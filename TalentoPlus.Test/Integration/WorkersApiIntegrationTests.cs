using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using TalentoPlusAPI;

namespace TalentoPlus.Test.Integration;

public class WorkersApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public WorkersApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetWorkers_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/workers");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8",
            response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task GetWorkers_ReturnsJsonArray()
    {
        // Act
        var response = await _client.GetAsync("/api/workers");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(content);
        Assert.StartsWith("[", content); // JSON array
    }
}

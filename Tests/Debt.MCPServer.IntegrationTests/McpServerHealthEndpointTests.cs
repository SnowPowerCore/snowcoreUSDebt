using Microsoft.AspNetCore.Mvc.Testing;

namespace Debt.MCPServer.IntegrationTests;

public class McpServerHealthEndpointTests : IClassFixture<McpServerWebApplicationFactory>
{
    private readonly HttpClient _client;

    public McpServerHealthEndpointTests(McpServerWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/health");

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task AliveEndpoint_ReturnsSuccess()
    {
        var response = await _client.GetAsync("/alive");

        Assert.True(response.IsSuccessStatusCode);
    }
}
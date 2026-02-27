using Debt.MCPServer.Interfaces;
using Debt.MCPServer.Models.Dto;
using MaybeResults;
using Microsoft.AspNetCore.Mvc.Testing;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;

namespace Debt.MCPServer.IntegrationTests;

public class McpServerToolsInteractionTests
{
    [Fact]
    public async Task ListToolsAsync_ContainsDateAndDebtTools()
    {
        using var factory = new McpServerWebApplicationFactory();
        await using var mcpClient = await CreateClientAsync(factory);

        var tools = await mcpClient.ListToolsAsync(cancellationToken: CancellationToken.None);

        Assert.Contains(tools, tool => tool.Name == "get_current_date");
        Assert.Contains(tools, tool => tool.Name == "get_us_debt");
    }

    [Fact]
    public async Task CallToolAsync_GetUsDebt_ReturnsStubbedPayload()
    {
        const string expectedPayload = "{\"stub\":\"debt-ok\"}";

        using var factory = new McpServerWebApplicationFactory(() => new StubDebtService(Maybe.Create(expectedPayload)));
        await using var mcpClient = await CreateClientAsync(factory);

        var result = await mcpClient.CallToolAsync(
            "get_us_debt",
            new Dictionary<string, object?>(),
            cancellationToken: CancellationToken.None);

        Assert.Equal(expectedPayload, ExtractText(result));
    }

    [Fact]
    public async Task CallToolAsync_GetUsDebt_ReturnsFallbackError_WhenServiceReturnsNonSome()
    {
        using var factory = new McpServerWebApplicationFactory(() => new StubDebtService((IMaybe<string>)null!));
        await using var mcpClient = await CreateClientAsync(factory);

        var result = await mcpClient.CallToolAsync(
            "get_us_debt",
            new Dictionary<string, object?>(),
            cancellationToken: CancellationToken.None);

        Assert.Equal(Resources.Resource.GetUsDebtError, ExtractText(result));
    }

    [Fact]
    public async Task CallToolAsync_GetCurrentDate_ReturnsStubbedUtcDateInExpectedFormat()
    {
        var fixedDate = new DateTimeOffset(2030, 1, 2, 14, 35, 0, TimeSpan.Zero);

        using var factory = new McpServerWebApplicationFactory(() => new StubDateTimeService(Maybe.Create(fixedDate)));
        await using var mcpClient = await CreateClientAsync(factory);

        var result = await mcpClient.CallToolAsync(
            "get_current_date",
            new Dictionary<string, object?>(),
            cancellationToken: CancellationToken.None);

        Assert.Equal("2030-01-02", ExtractText(result));
    }

    private static async Task<McpClient> CreateClientAsync(WebApplicationFactory<Program> factory)
    {
        var httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var transport = new HttpClientTransport(new() { Endpoint = httpClient.BaseAddress! }, httpClient);
        return await McpClient.CreateAsync(transport);
    }

    private static string ExtractText(CallToolResult result)
    {
        var text = result.Content
            .OfType<TextContentBlock>()
            .Select(content => content.Text)
            .FirstOrDefault();

        Assert.False(string.IsNullOrWhiteSpace(text));
        return text!;
    }

    private sealed class StubDebtService(IMaybe<string> result) : IDebtService
    {
        public Task<IMaybe<string>> GetUsDebtAsync(GetUsDebtArgs? args = null) => Task.FromResult(result);
    }

    private sealed class StubDateTimeService(IMaybe<DateTimeOffset> result) : IDateTimeService
    {
        public Task<IMaybe<DateTimeOffset>> GetCurrentDateTimeAsync() => Task.FromResult(result);
    }
}
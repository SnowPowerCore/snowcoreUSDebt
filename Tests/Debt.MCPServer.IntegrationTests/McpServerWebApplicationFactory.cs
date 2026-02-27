using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Debt.MCPServer.Interfaces;

namespace Debt.MCPServer.IntegrationTests;

public class McpServerWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly Func<IDebtService>? _debtServiceFactory;

    public McpServerWebApplicationFactory() { }

    internal McpServerWebApplicationFactory(Func<IDebtService> debtServiceFactory)
    {
        _debtServiceFactory = debtServiceFactory;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        if (_debtServiceFactory is null)
            return;

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDebtService>();
            services.AddScoped(_ => _debtServiceFactory());
        });
    }
}
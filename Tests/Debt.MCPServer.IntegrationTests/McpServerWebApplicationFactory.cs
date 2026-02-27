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
    private readonly Func<IDateTimeService>? _dateTimeServiceFactory;

    public McpServerWebApplicationFactory() { }

    internal McpServerWebApplicationFactory(Func<IDebtService> debtServiceFactory)
    {
        _debtServiceFactory = debtServiceFactory;
    }

    internal McpServerWebApplicationFactory(Func<IDateTimeService> dateTimeServiceFactory)
    {
        _dateTimeServiceFactory = dateTimeServiceFactory;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        if (_debtServiceFactory is null && _dateTimeServiceFactory is null)
            return;

        builder.ConfigureTestServices(services =>
        {
            if (_debtServiceFactory is not null)
            {
                services.RemoveAll<IDebtService>();
                services.AddScoped(_ => _debtServiceFactory());
            }

            if (_dateTimeServiceFactory is not null)
            {
                services.RemoveAll<IDateTimeService>();
                services.AddScoped(_ => _dateTimeServiceFactory());
            }
        });
    }
}
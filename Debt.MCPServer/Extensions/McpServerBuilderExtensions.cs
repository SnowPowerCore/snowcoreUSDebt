using Debt.MCPServer.Features.DateTime;
using Debt.MCPServer.Features.Debt;
using Debt.MCPServer.Resources;
using Debt.ServiceDefaults;

namespace Debt.MCPServer.Extensions;

internal static class McpServerBuilderExtensions
{
    public static WebApplicationBuilder AddMcpTransportAndTools(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddMcpServer()
            .WithHttpTransport()
            .WithTools<DateTools>()
            .WithTools<DebtTools>();

        return builder;
    }

    public static WebApplicationBuilder AddMcpInfrastructure(this WebApplicationBuilder builder)
    {
        builder.WebHost.UseKestrelHttpsConfiguration();
        builder.AddServiceDefaults();

        return builder;
    }

    public static WebApplicationBuilder AddMcpApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IDateTimeService, GetCurrentDateService>();
        builder.Services.AddScoped<IDebtService, GetUsDebtService>();

        return builder;
    }

    public static WebApplicationBuilder AddMcpThirdPartyApis(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureTreasuryDebtApizrManagers(static opts =>
            opts.WithBaseAddress(Resource.TreasuryDebtApiUrl));

        return builder;
    }
}
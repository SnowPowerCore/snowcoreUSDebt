using Debt.MCPServer.Extensions;
using Debt.ServiceDefaults;

var builder = WebApplication.CreateSlimBuilder(args);

builder
    .AddMcpTransportAndTools()
    .AddMcpInfrastructure()
    .AddMcpApplicationServices()
    .AddMcpThirdPartyApis();

var app = builder.Build();
app.UseHttpsRedirection();

app.MapMcp();
app.MapDefaultEndpoints();

await app.RunAsync();
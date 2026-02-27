using Debt.PublicApi.Constants;

const string McpInspectorResourceName = "mcp-inspector";
const string McpInspectorVersion = "0.21.0";

var builder = DistributedApplication.CreateBuilder(args);

var debtMcpBackendProject = builder.AddProject<Projects.Debt_MCPServer>(ProjectConstants.Projects_Debt_MCPServer);

builder.AddMcpInspector(McpInspectorResourceName, options =>
    {
        options.InspectorVersion = McpInspectorVersion;
    })
    .WithMcpServer(debtMcpBackendProject, path: string.Empty);

builder.AddProject<Projects.Debt_ConsoleClient>(ProjectConstants.Projects_Debt_ConsoleClient)
    .WithReference(debtMcpBackendProject)
    .WaitFor(debtMcpBackendProject);

await builder.Build().RunAsync();
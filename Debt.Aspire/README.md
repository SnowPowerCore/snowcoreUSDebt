# Debt.Aspire

.NET Aspire AppHost project that orchestrates the local runtime for the debt chat solution.

This project is the **entry point for local development orchestration**. It starts the MCP server and console client together, wires service references, and exposes Aspire dashboard resources.

## What This Project Does

`Debt.Aspire` composes these resources:

- `debt-mcp-server` (`Debt.MCPServer`) — MCP backend exposing debt/date tools
- `debt-console-client` (`Debt.ConsoleClient`) — chat-based console app that depends on MCP server
- `mcp-inspector` (CommunityToolkit Aspire integration) — MCP inspection/debug helper

Orchestration behavior implemented in `AppHost.cs`:

- Registers `Debt.MCPServer`
- Attaches MCP inspector to that server
- Registers `Debt.ConsoleClient`
- Adds `.WithReference(...)` and `.WaitFor(...)` so client starts after server is available

## Key Files

- `AppHost.cs` — distributed application composition
- `Debt.Aspire.csproj` — AppHost SDK + project/package references
- `Properties/launchSettings.json` — local HTTPS profile and Aspire environment variables
- `appsettings.json` / `appsettings.Development.json` — logging defaults

## Prerequisites

- .NET SDK 10.0 (target framework is `net10.0`)
- Local dev certificate configured for HTTPS (`dotnet dev-certs https --trust` if needed)
- Internet access (downstream services call Treasury Fiscal Data API)

## Run / Debug

### Recommended (IDE)

Open the solution in your IDE, right-click the `Debt.Aspire` project, then choose **Debug -> Start New Instance**.

### CLI

From repository root:

```bash
dotnet run --project Debt.Aspire
```

## Runtime Topology

At runtime, the AppHost:

1. Starts `debt-mcp-server`
2. Starts MCP inspector linked to the MCP server
3. Starts `debt-console-client` after MCP server readiness

This ensures the console app has its MCP dependency available before interaction begins.

## Configuration Notes

- Launch profile name: `https`
- `ASPNETCORE_ENVIRONMENT` and `DOTNET_ENVIRONMENT` default to `Development` in launch settings
- Aspire dashboard/resource endpoints are provided through launch profile environment variables

## Troubleshooting

- If HTTPS startup fails, re-trust dev certificates and restart IDE/terminal.
- If console client cannot reach MCP server, run via `Debt.Aspire` so startup ordering (`WaitFor`) is applied.
- If inspector/dashboard endpoints do not open, verify the generated localhost ports from `launchSettings.json` are not blocked.

## Related Projects

- `../Debt.MCPServer/` — MCP tools and Treasury API access
- `../Debt.ConsoleClient/` — chat UX and LLM interaction
- `../Debt.Universal/PublicApi/` — shared constants used by AppHost resource naming

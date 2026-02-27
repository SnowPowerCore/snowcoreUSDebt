# Tests

Consolidated test documentation for the solution.

This folder contains all automated tests for the debt console + MCP server solution, split by project responsibility (unit vs integration).

## Test Projects

## `Debt.ConsoleClient.Tests`

Type: unit tests for console client application/services.

Current focus areas:

- `ConsoleApplicationService`
  - app version initialization from version tracking
  - app shutdown behavior (`StopApplication` invocation)
- `ConsoleNavigationService`
  - screen navigation lifecycle calls
  - back navigation behavior with stack/disposal handling

Main dependencies:

- `xUnit`
- `Moq`
- `Microsoft.NET.Test.Sdk`
- `coverlet.collector`

## `Debt.MCPServer.Tests`

Type: unit tests for MCP server tool/services logic.

Current focus areas:

- `DateTools`
  - UTC date formatting success path
  - fallback error behavior on non-success result
- `DebtTools`
  - tool argument forwarding (`filter`, `sort`, `page_size`, `page_number`)
  - fallback error behavior on non-success result
- `GetCurrentDateService`
  - returns UTC `DateTimeOffset` wrapped in `Some`

Main dependencies:

- `xUnit`
- `Moq`
- `Microsoft.NET.Test.Sdk`
- `coverlet.collector`

## `Debt.MCPServer.IntegrationTests`

Type: integration tests for hosted MCP server behavior.

Current focus areas:

- MCP tool discovery (`get_current_date`, `get_us_debt`)
- MCP tool invocation end-to-end through HTTP transport
- behavior with stubbed service implementations (`IDebtService`, `IDateTimeService`)
- health/liveness endpoints (`/health`, `/alive`)

Main dependencies:

- `xUnit`
- `Microsoft.NET.Test.Sdk`
- `Microsoft.AspNetCore.Mvc.Testing`
- `coverlet.collector`

## Runtime / Prerequisites

- .NET SDK 10.0 (all test projects target `net10.0`)
- Restore completed from solution root

## Running Tests

From repository root.

Run all tests in solution:

```bash
dotnet test snowcoreUSDebt.slnx
```

Run only tests in this folder:

```bash
dotnet test Tests
```

Run a specific test project:

```bash
dotnet test Tests/Debt.ConsoleClient.Tests/Debt.ConsoleClient.Tests.csproj
dotnet test Tests/Debt.MCPServer.Tests/Debt.MCPServer.Tests.csproj
dotnet test Tests/Debt.MCPServer.IntegrationTests/Debt.MCPServer.IntegrationTests.csproj
```

Run with coverage collection:

```bash
dotnet test Tests --collect:"XPlat Code Coverage"
```

Filter by test name/class:

```bash
dotnet test Tests --filter "FullyQualifiedName~DebtToolsTests"
```

## Test Design Notes

- Unit test projects isolate behavior with mocks/stubs.
- Integration tests host the actual web app via `WebApplicationFactory<Program>`.
- Integration tests replace selected services through DI override when deterministic responses are required.
- Test outputs in `bin/` and `obj/` are generated artifacts and are not part of test source intent.

## Where to Add New Tests

- Console behavior/business flow changes: `Debt.ConsoleClient.Tests`
- MCP tool/service logic changes: `Debt.MCPServer.Tests`
- Endpoint/tool transport or host wiring changes: `Debt.MCPServer.IntegrationTests`

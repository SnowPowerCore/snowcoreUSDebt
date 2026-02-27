# Debt.MCPServer

MCP backend project for debt-domain tool execution.

`Debt.MCPServer` exposes the tool surface used by the console assistant and is the only component that talks to the Treasury Debt to the Penny API. It enforces transport/tool boundaries, maps API calls, and applies business-level result handling.

## Responsibilities

- Host MCP over HTTP transport.
- Expose exactly two debt-domain tools:
  - `get_current_date`
  - `get_us_debt`
- Fetch debt data only from Treasury Debt to the Penny API.
- Apply resilience and serialization configuration for external API calls.
- Return business outcomes through typed error-aware service results.

## Runtime Composition

`Program.cs` configures the web app through extension methods:

1. `AddMcpTransportAndTools()`
   - Registers MCP server
   - Enables HTTP transport
   - Registers tool classes `DateTools` and `DebtTools`
2. `AddMcpInfrastructure()`
   - Kestrel HTTPS configuration
   - Shared service defaults
3. `AddMcpApplicationServices()`
   - `IDateTimeService` -> `GetCurrentDateService`
   - `IDebtService` -> `GetUsDebtService`
4. `AddMcpThirdPartyApis()`
   - Registers Apizr manager for Treasury API
   - Uses configured base URL from resources

`app.MapMcp()` publishes the MCP endpoint.

## MCP Tools (Detailed)

## `get_current_date`

- Implemented in `Features/DateTime/DateTools.cs`
- Description: returns today’s date in UTC with format `yyyy-MM-dd`
- Backing service: `GetCurrentDateService`
- Success output: date string (example `2026-02-27`)
- Failure output: localized fallback message (`GetCurrentDateTimeError`)

## `get_us_debt`

- Implemented in `Features/Debt/DebtTools.cs`
- Description: fetches debt rows from Treasury Debt to the Penny API
- Backing service: `GetUsDebtService`

Arguments:

- `filter` (`string`, optional)
  - Passed to Treasury API `filter` query param
  - Example: `record_date:gte:2026-01-01`
- `sort` (`string`, optional)
  - Passed to Treasury API `sort` query param
  - Example: `-record_date`
- `page_size` (`int`, optional)
  - Mapped to API `page[size]`
- `page_number` (`int`, optional)
  - Mapped to API `page[number]`

Success output:

- JSON string representation of `TreasuryDebtResponse` (`data`, `meta`, `links`) serialized with source-generated context.

Failure output at MCP tool boundary:

- Generic localized message: `GetUsDebtError`.

## Business Logic Typed Errors

`GetUsDebtService` returns `Task<IMaybe<string>>` and represents failure reasons with typed domain errors before they are flattened to tool output.

Typed error types:

- `GetUsDebtApiError`
  - Used when Treasury API invocation fails (`response.IsSuccess == false`)
  - Captures upstream/API failure context
- `GetUsDebtNoDataForPeriodError`
  - Used when API call succeeds but no records are returned (`data == null || data.Count == 0`)
  - Represents valid request with unavailable dataset coverage for that period

Why this matters:

- Keeps business semantics explicit in the service layer.
- Distinguishes technical/API failure from “no data for requested period”.
- Enables future evolution where tool layer can map typed errors to different MCP responses without changing core fetch logic.

Note: current `DebtTools.GetUsDebtToolAsync` returns a single localized fallback (`GetUsDebtError`) for all non-success `IMaybe` outcomes. Typed errors still exist and are useful for internal logic separation and future response mapping improvements.

## Treasury API Integration

External API contract (`ITreasuryDebtApi`):

- Endpoint base: `https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v2/accounting/od/debt_to_penny`
- Method: `GET ""` (base endpoint)
- Query mapping:
  - `filter`
  - `sort`
  - `page[size]`
  - `page[number]`

Integration stack:

- Apizr + Refit manager registration
- Source-generated JSON serialization context (`DebtMcpJsonSerializerContext`)
- Standard resilience handler:
  - retry with jitter (up to 3 attempts)
  - attempt/total timeouts
  - circuit breaker sampling window

## Data Models

Primary DTOs in `Models/Dto`:

- `GetUsDebtArgs` — MCP-level argument model
- `TreasuryDebtResponse` — API envelope (`data`, `meta`, `links`)
- `TreasuryDebtRecord` — debt fields (record date, total outstanding debt, fiscal/calendar attributes, etc.)

## Configuration

- `Resources/Resource_*.json`
  - User-facing fallback/error messages
  - Date format string
  - Treasury base URL (`TreasuryDebtApiUrl`)
- `appsettings.json` / `appsettings.Development.json`
  - host logging setup

## Prerequisites

- .NET SDK 10.0 (`net10.0`)
- Network access to Treasury Fiscal Data API
- HTTPS-capable local development environment

## Run

From repository root:

```bash
dotnet run --project Debt.MCPServer
```

Recommended in full solution flow: start through `Debt.Aspire` so dependent projects are orchestrated together.

## Key Files

- `Program.cs` — host startup + endpoint mapping
- `Extensions/McpServerBuilderExtensions.cs` — MCP/services/API wiring
- `Features/DateTime/DateTools.cs` — `get_current_date`
- `Features/Debt/DebtTools.cs` — `get_us_debt`
- `Features/Debt/GetUsDebtService.cs` — business logic + typed errors
- `ErrorResults/GetUsDebtApiError.cs` — API failure business error type
- `ErrorResults/GetUsDebtNoDataForPeriodError.cs` — no-data business error type
- `Interfaces/Api/ITreasuryDebtApi.cs` — external API contract

# snowcoreUSDebt

Chat-based .NET solution for answering questions about **U.S. public debt** using only the U.S. Treasury **Debt to the Penny** dataset.

- Dataset page: https://fiscaldata.treasury.gov/datasets/debt-to-the-penny/debt-to-the-penny
- API endpoint: https://api.fiscaldata.treasury.gov/services/api/fiscal_service/v2/accounting/od/debt_to_penny

## Project Goal

Provide concise, factual answers about U.S. debt in a console chat experience, with short-term in-memory conversation context, and strict data/tool boundaries.

The design target is:

- Exactly 2 MCP tools available to the LLM:
  - `get_current_date`
  - `get_us_debt`
- `get_us_debt` is the **only** allowed data retrieval mechanism for debt values.
- No external datasets or APIs beyond Treasury Debt to the Penny.
- Out-of-scope questions should return a clear boundary response (for example: _"I don’t have expertise in that area."_).
- Missing/unsupported periods must be handled explicitly (for example: _"I don’t have data for that period."_).

## Solution Structure

- `Debt.ConsoleClient/` — Console chat application (LLM interaction and session flow).
- `Debt.MCPServer/` — MCP server exposing debt/date tools and integrating with Treasury API.
- `Debt.Aspire/` — .NET Aspire app host for local orchestration.
- `Debt.Universal/` — Shared/public API, service defaults, and reusable modules.
- `Tests/` — Unit and integration tests.

Main solution file: `snowcoreUSDebt.slnx`

## MCP Tools Contract

Current MCP tools exposed by `Debt.MCPServer`:

1. `get_current_date`
   - Returns current UTC date formatted as `yyyy-MM-dd`.

2. `get_us_debt`
   - Purpose: fetch debt rows from Treasury Debt to the Penny API.
   - Parameters:
     - `filter` (`string`, optional) — API filter expression, e.g. `record_date:gte:2026-01-01`
     - `sort` (`string`, optional) — sort expression, e.g. `-record_date`
     - `page_size` (`int`, optional) — page size
     - `page_number` (`int`, optional) — page number

## Prerequisites

- Windows, macOS, or Linux with terminal access
- .NET SDK 10.0 (projects target `net10.0`)
- Internet access to call Treasury Fiscal Data API
- (Optional) .NET Aspire workload/tools for AppHost workflows

## Install Workloads & Tooling

Use these commands if you plan to run the AppHost (`Debt.Aspire`) locally.

1. Check installed SDKs:

```bash
dotnet --list-sdks
```

2. Install Aspire workload:

```bash
dotnet workload install aspire
```

3. Verify installed workloads:

```bash
dotnet workload list
```

4. (Optional) update workloads:

```bash
dotnet workload update
```

Notes:

- If you only run projects individually (`Debt.MCPServer`, `Debt.ConsoleClient`), Aspire workload is not required.
- If HTTPS development certificates are missing/untrusted, run:

```bash
dotnet dev-certs https --trust
```

## Tested Environment

The repository is currently being worked on in:

- OS: Windows
- Runtime target: `net10.0`

If you test on another environment, please update this section with your validated setup.

## Getting Started

From repository root:

1. Restore:

```bash
dotnet restore snowcoreUSDebt.slnx
```

2. Build:

```bash
dotnet build snowcoreUSDebt.slnx
```

## Run Options

### Option A: Run with Aspire orchestration (recommended)

Open the solution in your IDE, right-click the `Debt.Aspire` project, then choose **Debug -> Start New Instance**.

This starts the MCP server and console client together under the AppHost orchestration.

### Option B: Run projects individually

Start MCP server:

```bash
dotnet run --project Debt.MCPServer
```

Then start console client in a second terminal:

```bash
dotnet run --project Debt.ConsoleClient
```

## Run Tests

```bash
dotnet test snowcoreUSDebt.slnx
```

## Behavioral Requirements (Reference)

Expected chat behaviors for this project:

- Supports debt lookups for specific periods when available in Treasury data.
- Supports analytical comparisons derived from dataset values (differences/growth/trends).
- Maintains memory only for the current process/session (RAM only; no persistence).
- Returns explicit messages when data is unavailable or outside supported period.
- Avoids speculation and filler; answers should remain concise and factual.
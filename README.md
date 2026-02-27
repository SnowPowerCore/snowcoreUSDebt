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

## AI Documentation

AI analysis and behavior assets are documented in:

- `.ai/README.md`
- `.ai/spec.md`
- `.ai/prompts/system.md`
- `.ai/tools/mcp-tools.md`
- `.ai/evals/test-cases.md`

## Behavioral Requirements (Reference)

Expected chat behaviors for this project:

- Supports debt lookups for specific periods when available in Treasury data.
- Supports analytical comparisons derived from dataset values (differences/growth/trends).
- Maintains memory only for the current process/session (RAM only; no persistence).
- Returns explicit messages when data is unavailable or outside supported period.
- Avoids speculation and filler; answers should remain concise and factual.

## Example Prompts to Try

Use these prompts in `Debt.ConsoleClient` to validate expected behavior.

### Core in-scope prompts (required)

- What was the U.S. debt in 2008?
- How much did the debt increase in 2024?
- What is the current U.S. debt?

### In-scope analytical prompts

- Compare U.S. debt between 2019 and 2020. Give absolute increase and percentage change.
- What was the month-over-month debt change from 2024-01-31 to 2024-02-29?
- Show the debt trend for Q1 2024 in a short bullet summary.

### Memory-in-conversation prompts (ask in sequence)

1. What was the U.S. debt in 2020?
2. Now compare it to 2021.
3. And what is the difference as a percentage of 2020?

Another sequence:

1. Give me the U.S. debt for 2023-12-29.
2. What about one year earlier?
3. Summarize the change in one sentence.

### Out-of-scope prompts (expected boundary response)

- Who won the 2024 U.S. presidential election?
- What is the weather in New York today?
- Give me stock picks for this week.

Expected response style: "I don't have expertise in that area."

### Corner cases and missing-data handling

- What was the debt in 1886?
- Give debt value for a date outside the dataset coverage.
- What was the debt on 2100-01-01?

Expected response style: "I don't have data for that period."

### Data/tool unavailability behavior

If Treasury data or tool access is unavailable during a query, the assistant should state this explicitly (for example: "I don't have data available right now.").

### Quality check for all answers

For all prompts above, answers should be:

- concise and factual
- derived only from Debt to the Penny dataset via MCP tools
- free of speculation, filler, or external-source claims
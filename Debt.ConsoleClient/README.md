# Debt.ConsoleClient

Console chat application for querying U.S. public debt through an LLM + MCP tools.

This project is the user-facing runtime of the solution. It hosts the interactive console experience, coordinates chat flow, maintains in-memory conversation context for the current run, and calls MCP tools through the configured LLM provider.

## Responsibilities

- Provide an interactive console UI for debt questions.
- Route startup through a step-based application launch pipeline.
- Model console "pages" as screens (`ScreenBase`, `ChatScreen`) with navigation.
- Keep chat memory in RAM only (per process/session).
- Use LLM tool-calling against MCP server tools for data-backed answers.

## Architecture Overview

### Composition Root

`Program.cs` builds the host and wires:

- Configuration loading from embedded `Config/*.json` + `appsettings.json`
- Core console/navigation/application services
- Telemetry and version tracking services
- Application launch worker (`UseStepifiedSystem` + hosted launch worker)
- Screen registration and default route (`ChatScreen`)
- LLM client registration (`ILlmClient` -> `CopilotLlmClient`)
- Named HTTP client for MCP server (`debt-mcp-server`)

### Step-Based Initialization (Application Launch)

Launch is implemented by `DebtConsoleAppLaunchService` using `MinimalStepifiedSystem`:

- `EveryTimeLaunch` is a stepified process with ordered steps:
  1. `HandleLaunchErrorsStep`
  2. `NavigateToRootScreenStep`

What each step does:

- `HandleLaunchErrorsStep`
  - Wraps launch execution in `try/catch`
  - Tracks exception telemetry and returns a safe result
- `NavigateToRootScreenStep`
  - Navigates to root screen through `IConsoleNavigationService`

Version tracking is initialized before launch execution and can be extended later for first-run/after-update flows.

Why this Stepified approach is useful when launch logic evolves over time:

- **Logic splitting**
  - Each concern lives in a focused step (error handling, navigation, future first-run/update logic), which keeps responsibilities isolated and readable.
- **Logic unitifying**
  - Complex, dependency-heavy behavior is turned into manageable units (steps), each requiring only the dependencies needed for that specific unit of logic.
- **Faster logic introduction**
  - New behavior is added as a new step and inserted into the pipeline, reducing impact on existing code and lowering regression risk when requirements change.

### Screen Model (Console "Pages")

Screens are the console equivalent of pages/views.

- `ScreenBase`
  - Base implementation of `IConsoleScreen`
  - Provides lifecycle hooks (`OnScreenAppearingAsync`, `OnScreenDisappearingAsync`)
  - Manages command loop and built-in commands (`help`, `exit`)
  - Handles welcome text and command rendering

- `ChatScreen`
  - Main interactive page for Q&A
  - Initializes MCP client and discovers tools from MCP server
  - Starts an LLM session and forwards user prompts
  - Processes asynchronous LLM events (assistant messages + idle notification)
  - Maintains chat history in-memory (`List<ChatMessage>`) for current process only

Navigation is stack-based (`ConsoleNavigationService`) and supports root navigation, forward navigation by route name, and back navigation with disposal.

## LLM Abstraction for Flexibility

The LLM integration is intentionally abstracted so providers can be swapped with minimal impact.

Core abstractions:

- `ILlmClient`
  - `StartAsync(...)`
  - `CreateSessionAsync(tools, ...)`
- `ILlmSession`
  - `SendAsync(prompt, ...)`
  - `On(Action<LlmSessionEvent>)`

Events are normalized through `LlmSessionEvent` derivatives (assistant output and session-idle signals), so `ChatScreen` depends on stable app-level events rather than provider-specific SDK event types.

Current implementation:

- `CopilotLlmClient` (GitHub Copilot SDK)
  - Reads token from `GH_TOKEN`
  - Uses configured model from `LlmSessionOptions` (current default: `gpt-5-mini`)
  - Bridges provider events into app events
  - Accepts MCP tools as `AIFunction` and forwards `McpClientTool` entries to session configuration

To replace provider, implement `ILlmClient`/`ILlmSession` and keep the rest of the app unchanged.

## Policy and Data Boundaries

The chat policy prompt (resources) enforces:

- Domain: U.S. public debt only
- Tools: only `get_current_date` and `get_us_debt`
- Out-of-scope response: "I don't have expertise in that area."
- Missing-period response: "I don't have data for that period."
- Data/tool failure response: "I don't have data available right now."
- Concise, factual responses and calculations only from tool-returned data

## Configuration

- `Resources/Resource_*.json`
  - UI strings, command names, and policy template
  - LLM defaults (`GithubCopilotModelName`, `GithubEnvVarName`)
- `Config/*.json` and `appsettings.json`
  - Loaded as embedded configuration at startup

## Prerequisites

- .NET SDK 10.0 (`net10.0`)
- `GH_TOKEN` set in environment for GitHub Copilot SDK access
- Reachable MCP server endpoint (typically orchestrated by `Debt.Aspire`)

## Run

Recommended: run through `Debt.Aspire` so MCP server dependency/start order is handled automatically.

Direct run (if MCP server is already available):

```bash
dotnet run --project Debt.ConsoleClient
```

## Key Files

- `Program.cs` — host composition and registration
- `Extensions/ServiceCollectionExtensions.cs` — DI wiring
- `Features/AppLaunch/DebtConsoleAppLaunchService.cs` — step-based launch process
- `Features/AppLaunch/EveryTime/*` — launch steps
- `Features/Base/ScreenBase.cs` — base screen/page behavior
- `Features/Chat/ChatScreen.cs` — chat page implementation
- `Features/LLM/Base/*` — provider-agnostic abstractions
- `Features/LLM/Copilot/*` — concrete Copilot adapter

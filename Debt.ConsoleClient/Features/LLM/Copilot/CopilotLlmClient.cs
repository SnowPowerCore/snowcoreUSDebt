using GitHub.Copilot.SDK;

namespace Debt.ConsoleClient.Features.LLM.Copilot;

public sealed partial class CopilotLlmClient : ILlmClient
{
    private readonly CopilotClient _client;
    private readonly LlmSessionOptions _sessionOptions;

    public CopilotLlmClient(IOptions<LlmSessionOptions> sessionOptions)
    {
        _sessionOptions = sessionOptions.Value;
        _client = new CopilotClient(new CopilotClientOptions
        {
            GitHubToken = Environment.GetEnvironmentVariable(Resource.GithubEnvVarName)
        });
    }

    public Task StartAsync(CancellationToken cancellationToken) =>
        _client.StartAsync(cancellationToken);

    public async Task<ILlmSession> CreateSessionAsync(IReadOnlyCollection<AIFunction> tools, CancellationToken cancellationToken)
    {
        var sessionConfig = new SessionConfig
        {
            Model = _sessionOptions.Model,
            Tools = []
        };

        foreach (var tool in tools)
        {
            if (tool is McpClientTool mcpClientTool)
            {
                sessionConfig.Tools.Add(mcpClientTool);
            }
        }

        var session = await _client.CreateSessionAsync(sessionConfig, cancellationToken);

        return new DelegatingLlmSession(
            onEvent => session.On(evt => ForwardEvent(evt, onEvent)),
            (prompt, ct) => session.SendAsync(new MessageOptions { Prompt = prompt }, ct),
            session.DisposeAsync);
    }

    public ValueTask DisposeAsync() =>
        _client.DisposeAsync();

    private static void ForwardEvent(SessionEvent evt, Action<LlmSessionEvent> onEvent)
    {
        switch (evt)
        {
            case AssistantMessageEvent assistantMessageEvent:
                onEvent(new AssistantMessageLlmSessionEvent(assistantMessageEvent.Data.Content));
                break;
            case SessionIdleEvent:
                onEvent(new SessionIdleLlmSessionEvent());
                break;
        }
    }
}
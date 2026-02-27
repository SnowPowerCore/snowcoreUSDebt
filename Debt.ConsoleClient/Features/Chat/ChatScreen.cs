using System.Text;
using System.Threading.Channels;
using Debt.PublicApi.Constants;

namespace Debt.ConsoleClient.Features.Chat;

public class ChatScreen : ScreenBase, IAsyncDisposable
{
    private const string ChatExitInput1 = "exit";
    private const string ChatExitInput2 = "quit";

    private bool _disposed = false;
    private readonly Lock _lock = new();
    private CancellationTokenSource _cancellationTokenSource = new();
    private Task<Task>? _workerTask;
    private TaskCompletionSource<bool> _idleTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
    
    private readonly HttpClient _httpClient;
    private McpClient _mcpClient = default!;
    private readonly ILlmClient _llmClient;

    private readonly Channel<LlmSessionEvent> _eventsChannel = Channel.CreateUnbounded<LlmSessionEvent>();
    private readonly List<ChatMessage> _history = [];
    private IList<McpClientTool> _tools = Array.Empty<McpClientTool>();

    public ChatScreen(IConsoleApplicationService application,
                      IHttpClientFactory httpClientFactory,
                      ILlmClient llmClient) : base(application)
    {
        _httpClient = httpClientFactory.CreateClient(ProjectConstants.Projects_Debt_MCPServer);
        _llmClient = llmClient;

        Commands.Add(Resource.ExitCommand2, new(ExitAsync, Resource.ExitCommand2Description));
        Commands.Add(Resource.ChatInitCommand, new(ChatInitAsync, Resource.ChatInitCommandDescription));
    }

    protected override async Task InitAsync()
    {
        _cancellationTokenSource = new();
        var clientTransport = new HttpClientTransport(new() { Endpoint = _httpClient.BaseAddress! }, _httpClient);
        _mcpClient = await McpClient.CreateAsync(clientTransport);
        _tools = await _mcpClient.ListToolsAsync(cancellationToken: CancellationToken.None);
    }

    private async Task ChatInitAsync()
    {
        await _llmClient.StartAsync(_cancellationTokenSource.Token);

        Application.Infrastructure.Console.PrintLine(Resource.ChatInitMessage);

        // Use a linked token so the screen-level cancellation cancels the chat loop.
        using var loopCts = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token);

        lock (_lock)
        {
            _workerTask ??= Task.Factory
                .StartNew(
                    ProcessIncomingEventsAsync,
                    loopCts.Token,
                    TaskCreationOptions.LongRunning | TaskCreationOptions.RunContinuationsAsynchronously | TaskCreationOptions.DenyChildAttach,
                    TaskScheduler.Default
                );
        }

        await using var session = await _llmClient.CreateSessionAsync(_tools.Cast<AIFunction>().ToList(), _cancellationTokenSource.Token);
        session.On(evt => _eventsChannel.Writer.TryWrite(evt));
        
        // Drain any leftover events from previous sessions
        while (_eventsChannel.Reader.TryRead(out _)) { }

        // Main input loop: read lines from the console, send to the session,
        // then wait until the session reports idle via a SessionIdleEvent.
        try
        {
            while (true)
            {
                if (loopCts.Token.IsCancellationRequested)
                    break;
                
                Application.Infrastructure.Console.PrintLine(Resource.ChatExitInfoMessage);
                Application.Infrastructure.Console.Print(Resource.ChatInputInfoMessage);
                var input = Application.Infrastructure.Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                // Check for local exit commands before sending to the session
                var trimmed = input.Trim();
                if (trimmed is ChatExitInput1 or ChatExitInput2)
                {
                    Application.Infrastructure.Console.PrintLine(Resource.ChatExitMessage);
                    break;
                }

                // Reset idle signal prior to sending new input
                _idleTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

                try
                {
                    _history.Add(new(ChatRole.User, input));
                    var prompt = BuildPrompt();
                    await session.SendAsync(prompt, loopCts.Token);
                }
                catch (OperationCanceledException) when (loopCts.Token.IsCancellationRequested)
                {
                    Application.Infrastructure.Console.PrintLine(Resource.ChatUserCancelledMessage);
                    break;
                }
                catch (Exception ex)
                {
                    Application.Infrastructure.Console.PrintLine(
                        string.Format(Resource.ChatErrorSendingMessage, ex.Message));
                    continue;
                }

                // Wait for the session to become idle or cancellation
                var completedTask = await Task.WhenAny(_idleTcs.Task, Task.Delay(TimeSpan.FromSeconds(120), loopCts.Token));
                if (completedTask != _idleTcs.Task)
                {
                    // Cancellation occurred
                    break;
                }
            }
        }
        finally
        {
            // ensure we cancel the loop if disposing
            loopCts.Cancel();
        }
    }

    public override async Task OnScreenDisappearingAsync()
    {
        await StopBackgroundWorkAsync();
    }

    private async Task ProcessIncomingEventsAsync()
    {
        await foreach (var evt in _eventsChannel.Reader.ReadAllAsync())
        {
            switch (evt)
            {
                case AssistantMessageLlmSessionEvent msg:
                    var response = msg.Content;
                    Application.Infrastructure.Console.PrintLine(response);
                    _history.Add(new(ChatRole.Assistant, response));
                    break;
                case SessionIdleLlmSessionEvent:
                    // Signal that the session is idle so the input loop can continue
                    _idleTcs.TrySetResult(true);
                    break;
            }
        }
    }

    private string BuildPrompt()
    {
        var builder = new StringBuilder();
        builder.AppendLine(
            string.Format(
                Resource.ChatSystemPolicyTemplate,
                Resource.ChatOutOfScopeResponse,
                Resource.ChatMissingPeriodResponse,
                Resource.ChatDatasetUnavailableResponse));
        builder.AppendLine();
        builder.AppendLine(Resource.ChatConversationHistoryHeader);

        foreach (var entry in _history)
        {
            builder.Append(entry.Role);
            builder.Append(": ");
            builder.AppendLine(entry.Text ?? string.Empty);
        }

        builder.AppendLine();
        builder.AppendLine(Resource.ChatLatestUserInstruction);
        return builder.ToString();
    }

    private async Task StopBackgroundWorkAsync()
    {
        _cancellationTokenSource?.Cancel();
        _eventsChannel?.Writer?.TryComplete();

        if (_workerTask is not default(Task<Task>))
        {
            await Task.WhenAny(_workerTask.Unwrap(), Task.Delay(TimeSpan.FromSeconds(5)));
        }
    }

    public async ValueTask DisposeAsync()
    {
        // Do not change this code. Put cleanup code in 'DisposeAsync(bool disposing)' method
        await DisposeAsync(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Stop background work (cancel, complete channel, await worker)
                await StopBackgroundWorkAsync();

                if (_mcpClient is not default(McpClient))
                {
                    await _mcpClient.DisposeAsync();
                }

                if (_llmClient is not default(ILlmClient))
                {
                    await _llmClient.DisposeAsync();
                }

                _cancellationTokenSource?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposed = true;
        }
    }
}
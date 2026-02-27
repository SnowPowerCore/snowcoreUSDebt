namespace Debt.ConsoleClient.Features.LLM.Copilot;

public sealed partial class CopilotLlmClient
{
    private sealed class DelegatingLlmSession(Action<Action<LlmSessionEvent>> subscribe,
                                              Func<string, CancellationToken, Task> sendAsync,
                                              Func<ValueTask> disposeAsync) : ILlmSession
    {
        public void On(Action<LlmSessionEvent> onEvent) =>
            subscribe(onEvent);

        public Task SendAsync(string prompt, CancellationToken cancellationToken) =>
            sendAsync(prompt, cancellationToken);

        public ValueTask DisposeAsync() =>
            disposeAsync();
    }
}
namespace Debt.ConsoleClient.Features.LLM.Base;

public interface ILlmSession : IAsyncDisposable
{
    void On(Action<LlmSessionEvent> onEvent);

    Task SendAsync(string prompt, CancellationToken cancellationToken);
}

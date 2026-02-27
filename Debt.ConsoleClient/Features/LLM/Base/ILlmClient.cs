namespace Debt.ConsoleClient.Features.LLM.Base;

public interface ILlmClient : IAsyncDisposable
{
    Task StartAsync(CancellationToken cancellationToken);

    Task<ILlmSession> CreateSessionAsync(IReadOnlyCollection<AIFunction> tools, CancellationToken cancellationToken);
}

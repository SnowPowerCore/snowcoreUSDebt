namespace Debt.ConsoleClient.Interfaces;

public interface IConsoleApplicationService
{
    IConsoleApplicationInfrastructureService Infrastructure { get; }

    ITelemetryService Logging { get; }

    Version AppVersion { get; }

    void Stop();
}
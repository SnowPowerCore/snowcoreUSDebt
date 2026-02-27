namespace Debt.ConsoleClient.Services;

public class ConsoleApplicationService(IHostApplicationLifetime hostLifetime,
                                       IConsoleApplicationInfrastructureService applicationInfrastructure,
                                       ITelemetryService telemetry,
                                       IVersionTrackingService versionTracking) : IConsoleApplicationService
{
    public IConsoleApplicationInfrastructureService Infrastructure { get; } = applicationInfrastructure;

    public ITelemetryService Logging { get; } = telemetry;

    public Version AppVersion { get; } = new Version(versionTracking.CurrentVersion);

    public void Stop() =>
        hostLifetime.StopApplication();
}
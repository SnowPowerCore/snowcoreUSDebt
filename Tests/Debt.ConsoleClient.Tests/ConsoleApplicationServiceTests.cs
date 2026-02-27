using Debt.ConsoleClient.Interfaces;
using Debt.ConsoleClient.Services;
using Debt.TelemetryHandling.Interfaces;
using Debt.VersionTracking.Interfaces;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Debt.ConsoleClient.Tests;

public class ConsoleApplicationServiceTests
{
    private readonly Mock<IHostApplicationLifetime> _hostLifetime;
    private readonly Mock<IConsoleApplicationInfrastructureService> _infrastructure;
    private readonly Mock<ITelemetryService> _telemetry;
    private readonly Mock<IVersionTrackingService> _versionTracking;

    private readonly ConsoleApplicationService _application;

    public ConsoleApplicationServiceTests()
    {
        _hostLifetime = new Mock<IHostApplicationLifetime>(MockBehavior.Strict);
        _infrastructure = new Mock<IConsoleApplicationInfrastructureService>(MockBehavior.Strict);
        _telemetry = new Mock<ITelemetryService>(MockBehavior.Strict);
        _versionTracking = new Mock<IVersionTrackingService>(MockBehavior.Strict);
        _versionTracking.SetupGet(v => v.CurrentVersion).Returns("1.0.0.0");

        _application = new ConsoleApplicationService(_hostLifetime.Object, _infrastructure.Object, _telemetry.Object, _versionTracking.Object);
    }

    [Fact]
    public void Constructor_SetsAppVersionFromVersionTracking()
    {
        _versionTracking.SetupGet(v => v.CurrentVersion).Returns("2.4.6.0");

        var sut = new ConsoleApplicationService(_hostLifetime.Object, _infrastructure.Object, _telemetry.Object, _versionTracking.Object);

        Assert.Equal(new Version("2.4.6.0"), sut.AppVersion);
    }

    [Fact]
    public void Stop_CallsHostStopApplication()
    {
        _hostLifetime.Setup(lifetime => lifetime.StopApplication());

        _application.Stop();

        _hostLifetime.Verify(lifetime => lifetime.StopApplication(), Times.Once);
    }
}
using Debt.ApplicationLaunch.Interfaces;
using Debt.ConsoleClient.Features.AppLaunch.EveryTime;
using MinimalStepifiedSystem.Attributes;

namespace Debt.ConsoleClient.Features.AppLaunch;

public class DebtConsoleAppLaunchService : IApplicationLaunchService
{
    private readonly IVersionTrackingService _versionTracking;

    [StepifiedProcess(Steps = [
        typeof(HandleLaunchErrorsStep),
        typeof(NavigateToRootScreenStep),
    ])]
    protected LaunchDelegate EveryTimeLaunch { get; }

    public DebtConsoleAppLaunchService(IVersionTrackingService versionTracking)
    {
        _versionTracking = versionTracking;

        _versionTracking.Track();
    }

    public async Task InitAsync()
    {
        var successCurrent = Version.TryParse(_versionTracking.CurrentVersion, out var currentVersion);
        var successPrevious = Version.TryParse(_versionTracking.PreviousVersion, out var previousVersion);

        var launchContext = new LaunchContext(currentVersion!);

        if (_versionTracking.IsFirstLaunchEver)
        {
            //await FirstTimeLaunch(launchContext);
        }

        if (successCurrent && successPrevious && currentVersion!.CompareTo(previousVersion) > 0)
        {
            //await AfterUpdateLaunch(launchContext);
        }

        await EveryTimeLaunch(launchContext);
    }
}
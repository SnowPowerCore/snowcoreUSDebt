namespace Debt.ConsoleClient.Features.AppLaunch.EveryTime;

public class NavigateToRootScreenStep(IConsoleNavigationService navigation) : IStep<LaunchDelegate, LaunchContext, ApplicationLaunchResult>
{
    public async Task<ApplicationLaunchResult> InvokeAsync(LaunchContext context, LaunchDelegate next, CancellationToken token = default)
    {
        await navigation.NavigateToRootAsync();
        return new();
    }
}
namespace Debt.ConsoleClient.Features.AppLaunch.EveryTime;

public class HandleLaunchErrorsStep(IConsoleApplicationService application) : IStep<LaunchDelegate, LaunchContext, ApplicationLaunchResult>
{
    public async Task<ApplicationLaunchResult> InvokeAsync(LaunchContext context, LaunchDelegate next, CancellationToken token = default)
    {
        try
        {
            return await next(context, token);
        }
        catch (Exception ex)
        {
            application.Logging.TrackException(ex);
            return new();
        }
    }
}
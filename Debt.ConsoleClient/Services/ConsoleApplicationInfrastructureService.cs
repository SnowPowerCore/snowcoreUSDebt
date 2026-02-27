namespace Debt.ConsoleClient.Services;

public class ConsoleApplicationInfrastructureService(IConsoleNavigationService navigation,
                                                     IConsoleService console) : IConsoleApplicationInfrastructureService
{
    public IConsoleNavigationService Navigation { get; } = navigation;

    public IConsoleService Console { get; } = console;
}
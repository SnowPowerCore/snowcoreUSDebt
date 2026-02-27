namespace Debt.ConsoleClient.Interfaces;

public interface IConsoleApplicationInfrastructureService
{
    IConsoleNavigationService Navigation { get; }

    IConsoleService Console { get; }
}
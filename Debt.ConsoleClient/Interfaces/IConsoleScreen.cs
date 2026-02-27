namespace Debt.ConsoleClient.Interfaces;

public interface IConsoleScreen
{
    Task InitScreenAsync();

    Task OnScreenAppearingAsync(DictionaryWithDefault<string, object>? args = null);

    Task OnScreenDisappearingAsync();
}
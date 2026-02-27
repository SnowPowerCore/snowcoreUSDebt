namespace Debt.ConsoleClient.Utils;

public record KnownScreenOptions
{
    public DictionaryWithDefault<string, Type> KnownScreenTypes { get; init; } =
        new(defaultValue: typeof(ScreenBase));

    public Type RootScreenType { get; set; } = typeof(ScreenBase);
}
using Debt.ConsoleHandling.Interfaces;

namespace Debt.ConsoleHandling.Implementations.Services;

/// <summary>
/// .NET Standard Console
/// </summary>
public class StandardConsoleService : IConsoleService
{
    public string? ReadLine() =>
        Console.ReadLine();

    public void PrintLine(string? text = null, ConsoleColor textColor = ConsoleColor.White)
    {
        if (textColor is not ConsoleColor.White)
            Console.ForegroundColor = textColor;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public void Print(string? text = null, ConsoleColor textColor = ConsoleColor.White)
    {
        if (textColor is not ConsoleColor.White)
            Console.ForegroundColor = textColor;
        Console.Write(text);
        Console.ResetColor();
    }

    public void New() =>
        SafeClear();

    private static void SafeClear()
    {
        try
        {
            if (!Console.IsOutputRedirected && !Console.IsInputRedirected)
            {
                Console.Clear();
                return;
            }

            for (var i = 0; i < 25; i++)
                Console.WriteLine();
        }
        catch { }
    }
}
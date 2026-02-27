namespace Debt.ConsoleClient.Utils;

public class ScreenCommand(Func<Task> execute,
                           string description = ScreenCommand.NoDescriptionCommand,
                           Action<Exception>? onException = null)
{
    private const string NoDescriptionCommand = "A command without description.";
    private long _isExecuting = 0;

    public string Description = description;

    public async Task ExecuteAsync()
    {
        if (CheckExecuting())
            return;

        Interlocked.Exchange(ref _isExecuting, 1);
        try
        {
            var taskToExecute = execute().ContinueWith(t =>
            {
                if (t.Status == TaskStatus.RanToCompletion
                    || t.Status == TaskStatus.Faulted
                    || t.Status == TaskStatus.Canceled)
                    Interlocked.Exchange(ref _isExecuting, 0);

                if (t.Status == TaskStatus.Faulted || t.Status == TaskStatus.Canceled)
                    onException?.Invoke(t.Exception!);

            });
            await Task.Run(() => taskToExecute);
        }
        catch (Exception)
        {
            Interlocked.Exchange(ref _isExecuting, 0);
        }
        finally
        {
            Interlocked.Exchange(ref _isExecuting, 0);
        }
    }

    public bool CheckExecuting() => Interlocked.Read(ref _isExecuting) != 0;
}
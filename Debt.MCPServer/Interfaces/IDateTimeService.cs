namespace Debt.MCPServer.Interfaces;

public interface IDateTimeService
{
    /// <summary>
    /// Gets the current date and time with timezone offset in ISO-8601 format, e.g. 2026-02-26T14:03:00-05:00.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the current date and time as a <see cref="DateTimeOffset"/>.</returns>
    Task<IMaybe<DateTimeOffset>> GetCurrentDateTimeAsync();
}
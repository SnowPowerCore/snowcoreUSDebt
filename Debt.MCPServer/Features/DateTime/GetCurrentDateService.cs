namespace Debt.MCPServer.Features.DateTime;

public class GetCurrentDateService : IDateTimeService
{
    /// <summary>
    /// Returns the current date and time as a DateTimeOffset. The time is in the local timezone of the server.
    /// </summary>
    /// <returns>A Task that resolves to a DateTimeOffset representing the current date and time with timezone offset.</returns>
    public Task<IMaybe<DateTimeOffset>> GetCurrentDateTimeAsync() =>
        Task.FromResult(Maybe.Create(DateTimeOffset.UtcNow));
}
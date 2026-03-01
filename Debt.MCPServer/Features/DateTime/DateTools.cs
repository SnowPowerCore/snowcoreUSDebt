namespace Debt.MCPServer.Features.DateTime;

[McpServerToolType]
public class DateTools(IDateTimeService dateTime)
{
    [McpServerTool(Name = "get_current_date"), Description("Returns today's date in UTC using format yyyy-MM-dd.")]
    public async Task<string> GetCurrentDateToolAsync()
    {
        var currentDateTimeResult = await dateTime.GetCurrentDateTimeAsync();
        if (currentDateTimeResult is not Some<DateTimeOffset> currentDateTime)
            return Resources.Resource.GetCurrentDateTimeError;
        return currentDateTime.Value.ToString(Resources.Resource.GetCurrentDateTimeFormat);
    }
}
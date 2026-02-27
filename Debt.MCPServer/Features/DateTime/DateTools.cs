namespace Debt.MCPServer.Features.DateTime;

[McpServerToolType]
public class DateTools(IDateTimeService dateTime)
{
    private const string GetCurrentDateToolName = "get_current_date";
    private const string GetCurrentDateToolDescription = "Returns today's date in UTC using format yyyy-MM-dd.";

    [McpServerTool(Name = GetCurrentDateToolName), Description(GetCurrentDateToolDescription)]
    public async Task<string> GetCurrentDateToolAsync()
    {
        var currentDateTimeResult = await dateTime.GetCurrentDateTimeAsync();
        if (currentDateTimeResult is not Some<DateTimeOffset> currentDateTime)
            return Resources.Resource.GetCurrentDateTimeError;
        return currentDateTime.Value.ToString(Resources.Resource.GetCurrentDateTimeFormat);
    }
}
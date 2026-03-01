namespace Debt.MCPServer.Features.Debt;

[McpServerToolType]
public class DebtTools(IDebtService debt)
{
    [McpServerTool(Name = "get_us_debt"), Description("Fetches US debt data from the Treasury API with optional filter, sort and pagination arguments.")]
    public async Task<string> GetUsDebtToolAsync(
        [Description("Optional API filter expression, e.g. record_date:gte:2026-01-01")] string filter = "",
        [Description("Optional sort expression, e.g. -record_date")] string sort = "",
        [Description("Optional page size")] int? pageSize = null,
        [Description("Optional page number")] int? pageNumber = null)
    {
        var args = new GetUsDebtArgs
        {
            Filter = filter,
            Sort = sort,
            PageSize = pageSize,
            PageNumber = pageNumber
        };

        var result = await debt.GetUsDebtAsync(args);
        if (result is not Some<string> currentDebt)
            return Resources.Resource.GetUsDebtError;

        return currentDebt.Value;
    }
}
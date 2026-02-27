namespace Debt.MCPServer.Features.Debt;

[McpServerToolType]
public class DebtTools(IDebtService debt)
{
    private const string GetUsDebtToolName = "get_us_debt";
    private const string GetUsDebtToolDescription = "Fetches US debt data from the Treasury API with optional filter, sort and pagination arguments.";
    private const string FilterArgDescription = "Optional API filter expression, e.g. record_date:gte:2026-01-01";
    private const string SortArgDescription = "Optional sort expression, e.g. -record_date";
    private const string PageSizeArgDescription = "Optional page size";
    private const string PageNumberArgDescription = "Optional page number";

    [McpServerTool(Name = GetUsDebtToolName), Description(GetUsDebtToolDescription)]
    public async Task<string> InvokeAsync(
        [Description(FilterArgDescription)] string filter = "",
        [Description(SortArgDescription)] string sort = "",
        [Description(PageSizeArgDescription)] int? pageSize = null,
        [Description(PageNumberArgDescription)] int? pageNumber = null)
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
namespace Debt.MCPServer.Interfaces.Api;

public interface ITreasuryDebtApi
{
    [Get("")]
    Task<IApiResponse<TreasuryDebtResponse>> GetDebtToPennyAsync(
        [AliasAs("filter")] string? filter = default,
        [AliasAs("sort")] string? sort = default,
        [AliasAs("page[size]")] int? pageSize = default,
        [AliasAs("page[number]")] int? pageNumber = default,
        CancellationToken cancellationToken = default);
}
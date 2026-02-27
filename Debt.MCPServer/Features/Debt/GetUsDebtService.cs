using Apizr;
using Debt.MCPServer.ErrorResults;

namespace Debt.MCPServer.Features.Debt;

public class GetUsDebtService(IApizrManager<ITreasuryDebtApi> apizrManager) : IDebtService
{
    public async Task<IMaybe<string>> GetUsDebtAsync(GetUsDebtArgs? args = null)
    {
        args ??= new GetUsDebtArgs();

        var response = await apizrManager.ExecuteAsync((options, api) =>
            api.GetDebtToPennyAsync(
                args.Filter,
                args.Sort,
                args.PageSize,
                args.PageNumber,
                options.CancellationToken));

        if (!response.IsSuccess)
            return GetUsDebtApiError<string>.Create(
                response.Exception?.Message ?? Resources.Resource.GetUsDebtError);

        if (response.Result?.Data is null || response.Result.Data.Count == 0)
            return GetUsDebtNoDataForPeriodError<string>.Create(Resources.Resource.GetUsDebtNoDataForPeriodError);

        return Maybe.Create(
            JsonSerializer.Serialize(response.Result, DebtMcpJsonSerializerContext.Default.TreasuryDebtResponse));
    }
}
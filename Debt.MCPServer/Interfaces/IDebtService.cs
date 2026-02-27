namespace Debt.MCPServer.Interfaces;

public interface IDebtService
{
    /// <summary>
    /// Gets the current US national debt in USD as a decimal number.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the current US national debt in USD.</returns>
    Task<IMaybe<string>> GetUsDebtAsync(GetUsDebtArgs? args = null);
}
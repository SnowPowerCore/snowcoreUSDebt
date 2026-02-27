namespace Debt.MCPServer.Models.Dto;

public sealed class TreasuryDebtResponse
{
    [JsonPropertyName("data")]
    public List<TreasuryDebtRecord> Data { get; init; } = [];

    [JsonPropertyName("meta")]
    public TreasuryDebtMeta? Meta { get; init; }

    [JsonPropertyName("links")]
    public TreasuryDebtLinks? Links { get; init; }
}
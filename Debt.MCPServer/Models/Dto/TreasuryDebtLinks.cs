namespace Debt.MCPServer.Models.Dto;

public sealed class TreasuryDebtLinks
{
    [JsonPropertyName("self")]
    public string? Self { get; init; }

    [JsonPropertyName("first")]
    public string? First { get; init; }

    [JsonPropertyName("prev")]
    public string? Prev { get; init; }

    [JsonPropertyName("next")]
    public string? Next { get; init; }

    [JsonPropertyName("last")]
    public string? Last { get; init; }
}
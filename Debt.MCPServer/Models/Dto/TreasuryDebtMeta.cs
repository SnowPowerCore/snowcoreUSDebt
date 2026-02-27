namespace Debt.MCPServer.Models.Dto;

public sealed class TreasuryDebtMeta
{
    [JsonPropertyName("count")]
    public int? Count { get; init; }

    [JsonPropertyName("total-count")]
    public int? TotalCount { get; init; }

    [JsonPropertyName("total-pages")]
    public int? TotalPages { get; init; }

    [JsonPropertyName("labels")]
    public Dictionary<string, string> Labels { get; init; } = [];

    [JsonPropertyName("dataTypes")]
    public Dictionary<string, string> DataTypes { get; init; } = [];

    [JsonPropertyName("dataFormats")]
    public Dictionary<string, string> DataFormats { get; init; } = [];
}
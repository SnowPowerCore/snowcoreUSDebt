namespace Debt.MCPServer.Models.Dto;

public sealed class GetUsDebtArgs
{
    [JsonPropertyName("filter")]
    public string? Filter { get; init; }

    [JsonPropertyName("sort")]
    public string? Sort { get; init; }

    [JsonPropertyName("page_size")]
    public int? PageSize { get; init; }

    [JsonPropertyName("page_number")]
    public int? PageNumber { get; init; }
}
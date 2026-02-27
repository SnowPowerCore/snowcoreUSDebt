namespace Debt.MCPServer.Models.Dto;

public sealed class TreasuryDebtRecord
{
    [JsonPropertyName("record_date")]
    public string? RecordDate { get; init; }

    [JsonPropertyName("debt_held_public_amt")]
    public string? DebtHeldPublicAmount { get; init; }

    [JsonPropertyName("intragov_hold_amt")]
    public string? IntragovHoldAmount { get; init; }

    [JsonPropertyName("tot_pub_debt_out_amt")]
    public string? TotalPublicDebtOutstandingAmount { get; init; }

    [JsonPropertyName("src_line_nbr")]
    public string? SourceLineNumber { get; init; }

    [JsonPropertyName("record_fiscal_year")]
    public string? RecordFiscalYear { get; init; }

    [JsonPropertyName("record_fiscal_quarter")]
    public string? RecordFiscalQuarter { get; init; }

    [JsonPropertyName("record_calendar_year")]
    public string? RecordCalendarYear { get; init; }

    [JsonPropertyName("record_calendar_quarter")]
    public string? RecordCalendarQuarter { get; init; }

    [JsonPropertyName("record_calendar_month")]
    public string? RecordCalendarMonth { get; init; }

    [JsonPropertyName("record_calendar_day")]
    public string? RecordCalendarDay { get; init; }
}
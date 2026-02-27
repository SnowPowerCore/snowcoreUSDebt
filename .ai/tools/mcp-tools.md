# MCP Tools Contract

This document defines the tool interface expected by the assistant.

## Allowed Tools (Exactly Two)

## 1) get_current_date

Purpose:

- Returns today's date in UTC.

Output:

- string formatted as yyyy-MM-dd

Notes:

- Use for "current" date resolution when needed.

## 2) get_us_debt

Purpose:

- Fetches debt data from Treasury Debt to the Penny API.

Parameters:

- filter (string, optional)
  - Treasury filter expression
  - example: record_date:gte:2026-01-01
- sort (string, optional)
  - sort expression
  - example: -record_date
- page_size (int, optional)
  - page size
- page_number (int, optional)
  - page number

Output:

- JSON payload representing Treasury API response (data/meta/links)

Usage rules:

- This is the only permitted source for debt values.
- Do not derive debt numbers from any external source.

## Expected Error Semantics

Service layer may classify failures as typed business outcomes:

- API failure (for example: GetUsDebtApiError)
- no data for requested period (for example: GetUsDebtNoDataForPeriodError)

Assistant-level user responses should map to:

- no data period -> I don't have data for that period.
- API/tool unavailable -> I don't have data available right now.

## Query Construction Tips

- For year-end lookup: filter by record_date range for target year, sort descending, page_size=1.
- For comparisons: fetch both periods explicitly, then compute deltas.
- For current debt: resolve current date, then query nearest available/latest relevant record.

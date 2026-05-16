namespace LedgerPro.Application.DTOs.Reports;

public record AccountSummaryReportDto(
    string FinancialYear,
    DateTime StartDate,
    DateTime EndDate,
    List<AccountSummaryRowDto> AccountSummaries,
    decimal TotalDebits,
    decimal TotalCredits
);

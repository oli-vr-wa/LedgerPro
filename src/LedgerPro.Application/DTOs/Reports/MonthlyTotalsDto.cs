
namespace LedgerPro.Application.DTOs.Reports;

public record MonthlyTotalsDto
{
    public int Year { get; init; }
    public int Month { get; init; }
    public decimal TotalRevenue { get; init; }
    public decimal TotalExpense { get; init; }
    public decimal TotalLiability { get; init; }
}

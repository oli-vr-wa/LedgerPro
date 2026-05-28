
namespace LedgerPro.Application.DTOs.Reports;

public class DashboardSummaryDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Assets { get; set; }
    public decimal Liabilities { get; set; }
    public int UnreconciledTransactionsCount { get; set; }    
}

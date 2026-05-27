using LedgerPro.Core.Enums;
using LedgerPro.Application.Interfaces;

namespace LedgerPro.Application.DTOs.Reports;

public class AccountSummaryRowDto
{
    public int AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public GeneralLedgerAccountType AccountType { get; set; }
    public decimal TotalDebits { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal Balance { get; set; }
}

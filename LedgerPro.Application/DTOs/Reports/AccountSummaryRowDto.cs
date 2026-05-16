using LedgerPro.Core.Enums;
using LedgerPro.Core.Interfaces;

namespace LedgerPro.Application.DTOs.Reports;

public class AccountSummaryRowDto : IAccountSummaryRowDto
{
    public int AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public GeneralLedgerAccountType AccountType { get; set; }
    public decimal TotalDebits { get; set; }
    public decimal TotalCredits { get; set; }
    public decimal Balance { get; set; }
}

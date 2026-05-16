using LedgerPro.Core.Interfaces;
using LedgerPro.Core.Enums;

namespace LedgerPro.Infrastructure.Projections;

public class GlAccountFinancialTotal : IGlAccountFinancialTotal
{
    public int AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public GeneralLedgerAccountType AccountType { get; set; }
    public decimal TotalDebits { get; set; }
    public decimal TotalCredits { get; set; }
}

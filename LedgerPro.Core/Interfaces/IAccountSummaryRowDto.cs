using LedgerPro.Core.Enums;

namespace LedgerPro.Core.Interfaces;

public interface IAccountSummaryRowDto
{
    int AccountId { get; set; }
    string AccountName { get; set; }
    GeneralLedgerAccountType AccountType { get; set; }
    decimal TotalDebits { get; set; }
    decimal TotalCredits { get; set; }
    decimal Balance { get; set; }    
}

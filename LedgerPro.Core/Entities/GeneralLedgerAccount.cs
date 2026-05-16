using LedgerPro.Core.Enums;

namespace LedgerPro.Core.Entities;

/// <summary>
/// Represents a general ledger account in the accounting system. Each account has a unique identifier, a name, a description, 
/// and a type that categorizes it as an Asset, Liability, Equity, Revenue, or Expense account.  
/// </summary>
public class GeneralLedgerAccount
{
    public int Id { get; set; } // e.g., 1000 for Assets, 2000 for Liabilities, etc.
    public string Name { get; set; } = string.Empty; // e.g., "Cash", "Accounts Payable", etc.
    public string Description { get; set; } = string.Empty; // Optional description of the account  
    public GeneralLedgerAccountType AccountType { get; set; } // Type of the account (Asset, Liability, Equity, Revenue, Expense)     

    // Navigation property for related GeneralLedgerItems
    public List<GeneralLedgerItem> GeneralLedgerItems { get; set; } = new List<GeneralLedgerItem>();   
}

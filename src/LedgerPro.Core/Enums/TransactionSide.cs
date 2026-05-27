namespace LedgerPro.Core.Enums;

/// <summary>
/// Represents the side of a transaction in double-entry accounting. A transaction can either be a Debit or a Credit, which affects how it 
/// impacts the general ledger accounts.
/// </summary>
public enum TransactionSide
{
    Debit,  // Represents an increase in assets or expenses, or a decrease in liabilities, equity, or revenue
    Credit  // Represents a decrease in assets or expenses, or an increase in liabilities, equity, or revenue
}

namespace LedgerPro.Core.Enums;

/// <summary>
/// Represents the status of a bank transaction in the context of the general ledger.
/// </summary>
public enum BankTransactionStatus
{
    Pending, // Transaction is pending and has not been processed yet
    Categorized, // Transaction has been categorized to a general ledger account but not yet reconciled
    Reconciled // Transaction has been reconciled with the general ledger
}

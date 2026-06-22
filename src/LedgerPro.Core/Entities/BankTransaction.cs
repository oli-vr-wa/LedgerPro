using LedgerPro.Core.Enums;

namespace LedgerPro.Core.Entities;

/// <summary>
/// Represents a bank transaction imported from a bank statement. 
/// Contains details about the transaction such as date, description, amount, type, and status. Also includes references to the bank source (account)
///  and the statement import it belongs to.
/// </summary>
public class BankTransaction : BaseGuidEntity
{
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = string.Empty; 
    public BankTransactionStatus Status { get; set; } = BankTransactionStatus.Pending;
    public int FinancialYearSequencialNumber { get; set; } // Sequential number for the transaction within the financial year, used for reference generation

    public Guid BankSourceId { get; set; }        // Reference to the bank source (account) this transaction belongs to
    public BankSource BankSource { get; set; } = null!;

    public Guid StatementImportId { get; set; }     // Reference to the statement import this transaction belongs to
    public StatementImport StatementImport { get; set; } = null!;

    // Navigation property to general ledger items associated with this transaction
    public virtual ICollection<GeneralLedgerItem> GeneralLedgerItems { get; set; } = new List<GeneralLedgerItem>(); 

    /// <summary>
    /// Adds a general ledger entry for this bank transaction based on the provided details. 
    /// The method determines the side of the transaction (debit or credit) based on the amount and generates a reference if not provided.
    /// </summary>
    /// <param name="glAccountId">The ID of the general ledger account.</param>
    /// <param name="amount">The amount of the transaction.</param>
    /// <param name="description">The description of the transaction.</param>
    /// <param name="reference">The reference for the general ledger item.</param>
    public void AddLedgerEntry(int glAccountId, decimal amount, string description, string? reference)
    {
        var ledgerItem = new GeneralLedgerItem
        {
            GeneralLedgerAccountId = glAccountId,
            Amount = Math.Abs(amount),
            Description = description,
            Reference = ResolveReference(reference),
            TransactionDate = TransactionDate,
            BankTransactionId = Id,
            Side = amount >= 0 ? TransactionSide.Debit : TransactionSide.Credit
        };

        GeneralLedgerItems.Add(ledgerItem);
    }

    /// <summary>
    /// Resolves the reference for a general ledger item. If a reference is provided, it returns it; otherwise, it generates a reference based on the bank transaction details.
    /// </summary>
    /// <param name="reference">The reference provided for the general ledger item.</param>
    /// <returns>The resolved reference for the general ledger item.</returns>
    private string ResolveReference(string? reference)
    {
        if (!string.IsNullOrWhiteSpace(reference))
            return reference;

        // Generate a reference based on the bank transaction ID and transaction date if not provided
        return $"BT-{TransactionDate:yyyyMMdd}-{FinancialYearSequencialNumber:D4}-{GeneralLedgerItems.Count + 1}";
    }
}

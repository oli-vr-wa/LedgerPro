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

    public Guid BankSourceId { get; set; }        // Reference to the bank source (account) this transaction belongs to
    public BankSource BankSource { get; set; } = null!;

    public Guid StatementImportId { get; set; }     // Reference to the statement import this transaction belongs to
    public StatementImport StatementImport { get; set; } = null!;
}

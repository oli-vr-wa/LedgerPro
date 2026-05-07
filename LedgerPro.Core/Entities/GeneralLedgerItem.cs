using LedgerPro.Core.Enums;

namespace LedgerPro.Core.Entities
{
    /// <summary>
    /// Represents a general ledger item in the accounting system. 
    /// Each item has a unique identifier, a transaction date, a reference, a description, an amount, and a side (debit or credit).
    /// </summary>
    public class GeneralLedgerItem
    {
        public Guid Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Reference { get; set; } = string.Empty; // e.g., invoice number, receipt number, etc.
        public string Description { get; set; } = string.Empty; // Detailed description of the transaction
        public decimal Amount { get; set; }
        public TransactionSide Side { get; set; } // Debit or Credit

        public int GeneralLedgerAccountId { get; set; } // Foreign key to the associated general ledger account
        public GeneralLedgerAccount GeneralLedgerAccount { get; set; } = null!; // Navigation property to the associated general ledger account

        public Guid? BankTransactionId { get; set; } // Foreign key to the associated bank transaction (if applicable)
        public BankTransaction? BankTransaction { get; set; } // Navigation property to the associated bank transaction (if applicable)
    }
}
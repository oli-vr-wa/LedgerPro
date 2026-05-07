namespace LedgerPro.Core.Entities
{
    /// <summary>
    /// Represents an import of a bank statement file. Contains metadata about the import and a collection of transactions imported from the statement.
    /// </summary>
    public class StatementImport : BaseGuidEntity
    {
        public string FileName { get; set; } = string.Empty;
        public DateTime ImportDate { get; set; }

        public string FileHash { get; set; } = string.Empty; // For duplicate detection

        public int TransactionCount { get; set; } // Number of transactions imported from this statement to display in history/logs

        public Guid BankSourceId { get; set; } // Reference to the bank source (account) this statement belongs to
        public virtual BankSource BankSource { get; set; } = null!; // Navigation property to the bank source

        public virtual ICollection<BankTransaction> Transactions { get; set; } = new List<BankTransaction>(); // Transactions imported from this statement
    }
}
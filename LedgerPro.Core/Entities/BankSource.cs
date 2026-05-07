namespace LedgerPro.Core.Entities;

/// <summary>
/// Represents a bank account or source of transactions. Contains details about the bank account and a collection of transactions associated with this source.
/// </summary>
public class BankSource : BaseGuidEntity
{   
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;  

    public virtual ICollection<BankTransaction> BankTransactions { get; set; } = new List<BankTransaction>();  // Navigation property to transactions associated with this bank source
    public virtual ICollection<StatementImport> StatementImports { get; set; } = new List<StatementImport>(); // Navigation property to statement imports associated with this bank source     
}

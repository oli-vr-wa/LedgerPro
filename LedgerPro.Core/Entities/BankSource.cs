namespace LedgerPro.Core.Entities;

public class BankSource
{
    public Guid Id { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;        
}

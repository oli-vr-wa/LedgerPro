using LedgerPro.Core.Enums;

namespace LedgerPro.Core.Entities
{
    public class BankTransaction
    {
        public Guid Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty; 
        public BankTransactionStatus Status { get; set; } = BankTransactionStatus.Pending;

        public Guid BankSourceId { get; set; }        
        public BankSource BankSource { get; set; } = null!;
    }
}
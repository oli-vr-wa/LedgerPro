using LedgerPro.Core.Entities;

namespace LedgerPro.Core.Interfaces
{
    /// <summary>
    /// Defines the contract for a repository that handles data access related to bank sources, transactions, and mappings. 
    /// This abstraction allows for separation of concerns and makes it easier to test the business logic without depending on the actual data access implementation.
    /// </summary>
    public interface IBankRepository
    {
        Task<BankSource?> GetBankSourceByIdAsync(Guid bankSourceId);
        Task<List<BankSource>> GetBankSourcesAsync();
        Task<List<BankTransactionMapping>> GetBankTransactionMappingsAsync();
        Task AddTransactionsAsync(IEnumerable<BankTransaction> transactions);
        Task AddGLItemsAsync(IEnumerable<GeneralLedgerItem> ledgerItems);
        Task AddStatementImportAsync(StatementImport statementImport);
        Task<int> SaveChangesAsync();
    }
}
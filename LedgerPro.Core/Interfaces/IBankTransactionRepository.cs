using LedgerPro.Core.Entities;

namespace LedgerPro.Core.Interfaces;

public interface IBankTransactionRepository
{
    Task<List<BankTransaction>> GetBankTransactionsAsync(Guid bankSourceId);
    Task AddTransactionsAsync(IEnumerable<BankTransaction> transactions);  
    Task<List<BankTransactionMapping>> GetBankTransactionMappingsAsync();
    Task<BankTransactionMapping> AddBankTransactionMappingAsync(BankTransactionMapping mapping);
    Task AddStatementImportAsync(StatementImport statementImport);
    Task<bool> IsBankTransactionMappingDuplicateAsync(BankTransactionMapping mapping);
}

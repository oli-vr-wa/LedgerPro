using LedgerPro.Core.Entities;

namespace LedgerPro.Core.Interfaces;

public interface IBankTransactionRepository
{
    Task<List<BankTransaction>> GetBankTransactionsAsync(Guid bankSourceId);
    Task AddTransactionsAsync(IEnumerable<BankTransaction> transactions);  
    Task<List<BankTransactionMapping>> GetBankTransactionMappingsAsync();
    Task AddBankTransactionMappingAsync(BankTransactionMapping mapping);
    Task AddStatementImportAsync(StatementImport statementImport);
}

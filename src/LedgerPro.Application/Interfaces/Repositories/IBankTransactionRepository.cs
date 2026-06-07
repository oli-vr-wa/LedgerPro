using LedgerPro.Application.DTOs.Reports;
using LedgerPro.Core.Entities;

namespace LedgerPro.Application.Interfaces.Repositories;

public interface IBankTransactionRepository
{
    Task<BankTransaction> GetBankTransactionByIdAsync(Guid bankTransactionId);
    Task<List<BankTransaction>> GetBankTransactionsAsync(Guid bankSourceId);
    Task AddTransactionsAsync(IEnumerable<BankTransaction> transactions);  
    Task<List<BankTransactionMapping>> GetBankTransactionMappingsAsync();
    Task<BankTransactionMapping> AddBankTransactionMappingAsync(BankTransactionMapping mapping);
    Task<BankTransactionMapping> UpdateBankTransactionMappingAsync(Guid id, BankTransactionMapping mapping);
    Task DeleteBankTransactionMappingAsync(Guid mappingId);
    Task AddStatementImportAsync(StatementImport statementImport);
    Task<bool> IsBankTransactionMappingDuplicateAsync(BankTransactionMapping mapping);
    Task<List<BankTransactionRowDto>> GetBankTransactionRowsAsync(Guid bankSourceId, int? financialYearEnding);
    Task<int> ReconcileBankTransactionAsync(BankTransaction bankTransaction, List<GeneralLedgerItem> generalLedgerItemsToAdd);
    Task ConfirmReconcileCategorizedBankTransactionAsync(BankTransaction bankTransaction, GeneralLedgerItem bankTransactionGlItem);
    Task UnreconcileBankTransactionAsync(BankTransaction bankTransaction);    
    Task<int> GetPendingReconciliationCountAsync(DateTime fromDate, DateTime toDate);
}

using LedgerPro.Application.DTOs.BankStatement;
using LedgerPro.Core.Common;
using LedgerPro.Core.Entities;

namespace LedgerPro.Application.Interfaces.Services;

public interface IBankTransactionService
{    
    Task AddBankTransactionMappingAsync(BankTransactionMapping mapping);  
    Task<Result<bool>> MatchPendingTransactionsAsync();
    Task<int> ReconcileBankTransactionAsync(ReconcileBankTransactionRequest request);  
    Task ConfirmReconcileCategorizedBankTransactionAsync(Guid bankTransactionId);
    Task UnreconcileBankTransactionAsync(Guid bankTransactionId);
}

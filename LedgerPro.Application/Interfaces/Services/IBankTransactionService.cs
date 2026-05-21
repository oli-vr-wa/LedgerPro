using LedgerPro.Application.DTOs.BankStatement;
using LedgerPro.Core.Entities;

namespace LedgerPro.Application.Interfaces.Services;

public interface IBankTransactionService
{    
    Task AddBankTransactionMappingAsync(BankTransactionMapping mapping);  
    Task<int> ReconcileBankTransactionAsync(ReconcileBankTransactionRequest request);  
    Task<int> ConfirmReconcileCategorizedBankTransactionAsync(Guid bankTransactionId);
    Task UnreconcileBankTransactionAsync(Guid bankTransactionId);
}

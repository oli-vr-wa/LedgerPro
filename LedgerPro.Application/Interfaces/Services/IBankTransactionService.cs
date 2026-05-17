using LedgerPro.Core.Entities;

namespace LedgerPro.Application.Interfaces.Services;

public interface IBankTransactionService
{
    Task AddBankTransactionMappingAsync(BankTransactionMapping mapping);    
}

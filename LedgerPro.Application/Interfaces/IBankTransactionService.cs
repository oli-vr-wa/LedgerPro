using LedgerPro.Core.Entities;

namespace LedgerPro.Application.Interfaces;

public interface IBankTransactionService
{
    Task AddBankTransactionMappingAsync(BankTransactionMapping mapping);    
}

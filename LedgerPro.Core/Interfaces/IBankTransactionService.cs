using LedgerPro.Core.Entities;

namespace LedgerPro.Core.Interfaces;

public interface IBankTransactionService
{
    Task AddBankTransactionMappingAsync(BankTransactionMapping mapping);    
}

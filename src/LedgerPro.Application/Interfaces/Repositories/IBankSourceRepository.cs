using LedgerPro.Application.DTOs.BankSource;
using LedgerPro.Application.DTOs.BankStatement;
using LedgerPro.Core.Entities;

namespace LedgerPro.Application.Interfaces.Repositories;

/// <summary>
/// Defines the contract for a repository that handles data access related to bank sources, transactions, and mappings. 
/// This abstraction allows for separation of concerns and makes it easier to test the business logic without depending on the actual data access implementation.
/// </summary>
public interface IBankSourceRepository
{
    Task<BankSource?> GetBankSourceByIdAsync(Guid bankSourceId);
    Task<List<BankSource>> GetBankSourcesAsync();
    Task AddBankSourceAsync(BankSource bankSource);   
    Task UpdateBankSourceAsync(Guid id, UpdateBankSourceRequest bankSource);
    Task DeleteBankSourceAsync(Guid bankSourceId);
    Task<bool> IsBankSourceNameInUseAsync(string name);
    Task<bool> IsBankSourceInUseAsync(Guid bankSourceId);
    Task<int> GetBankSourceGeneralLedgerAccountIdAsync(Guid bankSourceId);
    Task<IEnumerable<BankSourceTransactionsRow>> GetBankSourceTransactionsRowsAsync();
}

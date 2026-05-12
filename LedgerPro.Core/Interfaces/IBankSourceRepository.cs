using LedgerPro.Core.Entities;

namespace LedgerPro.Core.Interfaces;

/// <summary>
/// Defines the contract for a repository that handles data access related to bank sources, transactions, and mappings. 
/// This abstraction allows for separation of concerns and makes it easier to test the business logic without depending on the actual data access implementation.
/// </summary>
public interface IBankSourceRepository
{
    Task<BankSource?> GetBankSourceByIdAsync(Guid bankSourceId);
    Task<List<BankSource>> GetBankSourcesAsync();
    Task AddBankSourceAsync(BankSource bankSource);    
}

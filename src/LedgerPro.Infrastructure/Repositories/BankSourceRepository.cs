using LedgerPro.Core.Entities;
using LedgerPro.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using LedgerPro.Infrastructure.Extensions;
using LedgerPro.Application.DTOs.BankSource;

namespace LedgerPro.Infrastructure.Repositories;

public class BankSourceRepository(LedgerDbContext dbContext) : IBankSourceRepository
{
    private readonly LedgerDbContext _dbContext = dbContext;

    /// <summary>
    /// Retrieves a BankSource entity by its unique identifier. Returns null if no matching BankSource is found.
    /// </summary>
    /// <param name="bankSourceId">The unique identifier of the BankSource to retrieve.</param>
    /// <returns>The BankSource entity if found; otherwise, null.</returns>
    public async Task<BankSource?> GetBankSourceByIdAsync(Guid bankSourceId) =>                    
        await _dbContext.BankSources.FindAsync(bankSourceId);
    

    /// <summary>
    /// Adds a new BankSource entity to the database context. This method is used to create a new bank source that can be associated with imported bank statements.
    /// </summary>
    /// <param name="bankSource">The BankSource entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddBankSourceAsync(BankSource bankSource) =>
        await _dbContext.BankSources.AddAsync(bankSource);    
    
    /// <summary>
    /// Updates an existing BankSource entity in the database context. 
    /// This method is used to modify the details of an existing bank source, such as its name or connection settings.
    /// </summary>
    /// <param name="id">The unique identifier of the BankSource to update.</param>
    /// <param name="bankSource">The BankSource entity with updated values.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided ID is empty or the BankSource entity is null.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when no BankSource entity with the specified ID is found.</exception>
    public async Task UpdateBankSourceAsync(Guid id, UpdateBankSourceRequest dto)
    {
        if (id == Guid.Empty || dto == null)
            throw new ArgumentException("Invalid bank source ID or bank source entity.");
        if (dto == null)
            throw new ArgumentException("Bank source entity cannot be null.");

        var existingBankSource = await _dbContext.BankSources.FindAsync(id) ??
            throw new KeyNotFoundException($"Bank source with ID {id} not found.");
        
        existingBankSource.AccountName = dto.AccountName;
        existingBankSource.AccountNumber = dto.AccountNumber;
        existingBankSource.BankName = dto.BankName;
        existingBankSource.BankType = dto.BankType;
    }

    /// <summary>
    /// Deletes a BankSource entity from the database context based on its unique identifier. 
    /// This method is used to remove a bank source from the system, which may be necessary if the bank source is no longer needed or if it was added in error. 
    /// Note that this method does not check for related entities (e.g., transactions), this is handled at the service layer to ensure that bank sources that are in use cannot be 
    /// deleted without proper handling of related data.
    /// </summary>
    /// <param name="bankSourceId">The unique identifier of the BankSource to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided ID is empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when no BankSource entity with the specified ID is found.</exception>
    public async Task DeleteBankSourceAsync(Guid bankSourceId)
    {
        if (bankSourceId == Guid.Empty)
            throw new ArgumentException("Invalid bank source ID.");

        var bankSource = await _dbContext.BankSources.FindAsync(bankSourceId) ??
            throw new KeyNotFoundException($"Bank source with ID {bankSourceId} not found.");

        _dbContext.BankSources.Remove(bankSource);
    }

    /// <summary>
    /// Retrieves all BankSource entities from the database. This method is typically used to get a list of all bank sources 
    /// that have been configured in the system, allowing the application to display them or use them for selection when importing bank statements.
    /// </summary>
    /// <returns>All BankSource entities.</returns>
    public async Task<List<BankSource>> GetBankSourcesAsync() =>
        await _dbContext.BankSources.ToListAsync();
    
    /// <summary>
    /// Checks if a bank source name is already in use. 
    /// This method is used to ensure that bank source names are unique when adding new bank sources to the system.
    /// </summary>
    /// <param name="name">The name of the bank source to check.</param>
    /// <returns>True if the bank source name is already in use; otherwise, false.</returns>
    public async Task<bool> IsBankSourceNameInUseAsync(string name) =>
        await _dbContext.BankSources.AnyAsync(bs => bs.BankName == name);

    /// <summary>
    /// Checks if a bank source is currently in use by any bank transactions. 
    /// This method is used to determine if a bank source can be safely deleted without leaving orphaned transactions that reference it.
    /// </summary>
    /// <param name="bankSourceId">The unique identifier of the BankSource to check.</param>
    /// <returns>True if the bank source is in use; otherwise, false.</returns>
    public async Task<bool> IsBankSourceInUseAsync(Guid bankSourceId) {
        if (bankSourceId == Guid.Empty)
            throw new ArgumentException("Invalid bank source ID.");
        
        var bankSourceGlAccountId = await GetBankSourceGeneralLedgerAccountIdAsync(bankSourceId);

        bool isInUseOnBankTransactions = await _dbContext.BankTransactions.AnyAsync(t => t.BankSourceId == bankSourceId);
        // Additionally check if the bank source's GL account is referenced by any general ledger items, 
        // which would also indicate that the bank source is in use and cannot be deleted.
        bool isInUseOnGeneralLedgerItems = await _dbContext.GeneralLedgerItems.AnyAsync(t => t.GeneralLedgerAccountId == bankSourceGlAccountId);

        return isInUseOnBankTransactions || isInUseOnGeneralLedgerItems;
    }   

    /// <summary>
    /// Retrieves the general ledger account ID associated with a given bank source ID. 
    /// This method is used to find the GL account that corresponds to a specific bank source,
    /// which can be useful for various financial operations and validations.
    /// </summary>
    /// <param name="bankSourceId">The unique identifier of the bank source.</param>
    /// <returns>The ID of the associated general ledger account.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided ID is empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the bank source is not found.</exception>
    public async Task<int> GetBankSourceGeneralLedgerAccountIdAsync(Guid bankSourceId)
    {
        if (bankSourceId == Guid.Empty)
            throw new ArgumentException("Invalid bank source ID.");

        int glAccountId = await _dbContext.BankSources
            .Where(bs => bs.Id == bankSourceId)
            .Select(bs => bs.GeneralLedgerAccountId)
            .FirstOrDefaultAsync();

        if (glAccountId == 0)
            throw new KeyNotFoundException($"Bank source with ID {bankSourceId} not found.");

        return glAccountId;
    }     
}

using LedgerPro.Core.Entities;
using LedgerPro.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LedgerPro.Infrastructure.Repositories
{
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
        /// Retrieves all BankSource entities from the database. This method is typically used to get a list of all bank sources 
        /// that have been configured in the system, allowing the application to display them or use them for selection when importing bank statements.
        /// </summary>
        /// <returns>All BankSource entities.</returns>
        public async Task<List<BankSource>> GetBankSourcesAsync() =>
            await _dbContext.BankSources.ToListAsync();
    }
}
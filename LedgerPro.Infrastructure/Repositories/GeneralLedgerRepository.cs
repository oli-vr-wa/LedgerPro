using LedgerPro.Core.Entities;
using LedgerPro.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LedgerPro.Infrastructure.Repositories
{
    /// <summary>
    /// Implements the IGeneralLedgerRepository interface to provide data access methods for GeneralLedgerItem and GeneralLedgerAccount entities.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    public class GeneralLedgerRepository(LedgerDbContext dbContext) : IGeneralLedgerRepository
    {
        private readonly LedgerDbContext _dbContext = dbContext;

        /// <summary>
        /// Retrieves all GeneralLedgerItem entities from the database. 
        /// </summary>
        /// <returns>A list of GeneralLedgerItem entities.</returns>
        public async Task<List<GeneralLedgerItem>> GetGeneralLedgerItemsAsync() =>
            await _dbContext.GeneralLedgerItems.ToListAsync();

        /// <summary>
        /// Adds a collection of GeneralLedgerItem entities to the database context. This method is used 
        /// during the bank statement import process to add the generated GeneralLedgerItems (based on matched transactions) 
        /// to the context before saving them to the database.
        /// </summary>
        /// <param name="ledgerItems">The collection of GeneralLedgerItem entities to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddGeneralLedgerItemsAsync(IEnumerable<GeneralLedgerItem> ledgerItems) =>
            await _dbContext.GeneralLedgerItems.AddRangeAsync(ledgerItems);

        /// <summary>
        /// Adds a new GeneralLedgerAccount entity to the database context. 
        /// This method is used to create a new general ledger account that can be associated with GeneralLedgerItems for categorization and reporting purposes.
        /// </summary>
        /// <param name="glAccount">The GeneralLedgerAccount entity to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddGeneralLedgerAccountAsync(GeneralLedgerAccount glAccount) =>
            await _dbContext.GeneralLedgerAccounts.AddAsync(glAccount);
    }
}
using LedgerPro.Core.Entities;
using LedgerPro.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LedgerPro.Infrastructure.Repositories
{
    public class BankRepository : IBankRepository
    {
        private readonly LedgerDbContext _dbContext;

        public BankRepository(LedgerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Retrieves a BankSource entity by its unique identifier. Returns null if no matching BankSource is found.
        /// </summary>
        /// <param name="bankSourceId">The unique identifier of the BankSource to retrieve.</param>
        /// <returns>The BankSource entity if found; otherwise, null.</returns>
        public async Task<BankSource?> GetBankSourceByIdAsync(Guid bankSourceId)
        {
            //var allSources = await _dbContext.BankSources.ToListAsync();
            ///return allSources.FirstOrDefault(x => x.Id == bankSourceId);
            return await _dbContext.BankSources.FindAsync(bankSourceId);
        }
            
        
        /// <summary>
        /// Retrieves all BankSource entities from the database. This method is typically used to get a list of all bank sources 
        /// that have been configured in the system, allowing the application to display them or use them for selection when importing bank statements.
        /// </summary>
        /// <returns>All BankSource entities.</returns>
        public async Task<List<BankSource>> GetBankSourcesAsync() =>
            await _dbContext.BankSources.ToListAsync();

        /// <summary>
        /// Retrieves all BankTransactionMapping entities from the database. This method is typically used to get the mapping rules that are applied when importing 
        /// bank transactions, allowing the application to determine how to categorize and create GeneralLedgerItems based on the imported transactions.
        /// </summary>
        /// <returns>List of BankTransactionMapping entities.</returns>
        public async Task<List<BankTransactionMapping>> GetBankTransactionMappingsAsync() =>        
            await _dbContext.BankTransactionMappings.ToListAsync();
        
        /// <summary>
        /// Adds a StatementImport entity to the database context. This method is used during the bank statement import process to create a record of the import operation, 
        /// including details such as the bank source, import date, file hash, and transaction count. 
        /// The StatementImport record can be used for auditing and tracking purposes to keep a history of imported statements.
        /// </summary>
        /// <param name="statementImport"></param>
        /// <returns></returns>
        public async Task AddStatementImportAsync(StatementImport statementImport) =>
            await _dbContext.StatementImports.AddAsync(statementImport);

        /// <summary>
        /// Adds a collection of BankTransaction entities to the database context. This method is used during the bank statement import process to add the parsed transactions to the context before saving them to the database.
        /// </summary>
        /// <param name="transactions">The collection of BankTransaction entities to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddTransactionsAsync(IEnumerable<BankTransaction> transactions) =>
            await _dbContext.BankTransactions.AddRangeAsync(transactions);    

        /// <summary>
        /// Adds a collection of GeneralLedgerItem entities to the database context. This method is used 
        /// during the bank statement import process to add the generated GeneralLedgerItems (based on matched transactions) 
        /// to the context before saving them to the database.
        /// </summary>
        /// <param name="ledgerItems">The collection of GeneralLedgerItem entities to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddGLItemsAsync(IEnumerable<GeneralLedgerItem> ledgerItems) =>
            await _dbContext.GeneralLedgerItems.AddRangeAsync(ledgerItems);

        public async Task<int> SaveChangesAsync() =>
            await _dbContext.SaveChangesAsync();
    }
}
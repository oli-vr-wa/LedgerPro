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

        /// <summary>
        /// Retrieves all BankTransactionMapping entities from the database. This method is typically used to get the mapping rules that are applied when importing 
        /// bank transactions, allowing the application to determine how to categorize and create GeneralLedgerItems based on the imported transactions.
        /// </summary>
        /// <returns>List of BankTransactionMapping entities.</returns>
        public async Task<List<BankTransactionMapping>> GetBankTransactionMappingsAsync() =>        
            await _dbContext.BankTransactionMappings.ToListAsync();

        /// <summary>
        /// Adds a new BankTransactionMapping entity to the database context. This method is used to create a new mapping rule 
        /// that defines how certain bank transactions should be categorized and matched to GeneralLedgerItems during the import process.
        /// </summary>
        /// <param name="mapping">The BankTransactionMapping entity to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddBankTransactionMappingAsync(BankTransactionMapping mapping) =>
            await _dbContext.BankTransactionMappings.AddAsync(mapping);
        
        /// <summary>
        /// Adds a StatementImport entity to the database context. This method is used during the bank statement import process to create a record of the import operation, 
        /// including details such as the bank source, import date, file hash, and transaction count. 
        /// The StatementImport record can be used for auditing and tracking purposes to keep a history of imported statements.
        /// </summary>
        /// <param name="statementImport">The StatementImport entity to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddStatementImportAsync(StatementImport statementImport) =>
            await _dbContext.StatementImports.AddAsync(statementImport);

        /// <summary>
        /// Retrieves all BankTransaction entities associated with a specific BankSourceId. 
        /// </summary>
        /// <param name="bankSourceId">The ID of the bank source for which to retrieve transactions.</param>
        /// <returns>A list of BankTransaction entities.</returns>
        public async Task<List<BankTransaction>> GetBankTransactionsAsync(Guid bankSourceId) =>
            await _dbContext.BankTransactions.Where(t => t.BankSourceId == bankSourceId).ToListAsync();

        /// <summary>
        /// Adds a collection of BankTransaction entities to the database context. This method is used during the bank statement import process to add the parsed transactions to the context before saving them to the database.
        /// </summary>
        /// <param name="transactions">The collection of BankTransaction entities to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddTransactionsAsync(IEnumerable<BankTransaction> transactions) =>
            await _dbContext.BankTransactions.AddRangeAsync(transactions);    

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
        public async Task AddGLItemsAsync(IEnumerable<GeneralLedgerItem> ledgerItems) =>
            await _dbContext.GeneralLedgerItems.AddRangeAsync(ledgerItems);

        /// <summary>
        /// Adds a new GeneralLedgerAccount entity to the database context. 
        /// This method is used to create a new general ledger account that can be associated with GeneralLedgerItems for categorization and reporting purposes.
        /// </summary>
        /// <param name="glAccount">The GeneralLedgerAccount entity to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddGLAccountAsync(GeneralLedgerAccount glAccount) =>
            await _dbContext.GeneralLedgerAccounts.AddAsync(glAccount);

        public async Task<int> SaveChangesAsync() =>
            await _dbContext.SaveChangesAsync();
    }
}
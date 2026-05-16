using LedgerPro.Core.Entities;
using LedgerPro.Core.Exceptions;
using LedgerPro.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LedgerPro.Infrastructure.Repositories;

public class BankTransactionRepository(LedgerDbContext dbContext) : IBankTransactionRepository
{
    private readonly LedgerDbContext _dbContext = dbContext;
    
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
    /// Before adding the new mapping, the method checks for duplicates to prevent adding the same mapping multiple times. 
    /// If a duplicate is found, a BusinessException is thrown.
    /// </summary>
    /// <param name="mapping">The BankTransactionMapping entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<BankTransactionMapping> AddBankTransactionMappingAsync(BankTransactionMapping mapping) 
    {
        if (mapping == null)
            throw new ArgumentNullException(nameof(mapping), "The bank transaction mapping cannot be null.");

        bool isDuplicate = await IsBankTransactionMappingDuplicateAsync(mapping);

        if (isDuplicate)        
            throw new BusinessException("The bank transaction mapping already exists.");

        await _dbContext.BankTransactionMappings.AddAsync(mapping);        
        return mapping;
    }
    
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
    /// Checks if a given BankTransactionMapping already exists in the database to prevent duplicates. 
    /// This method is used before adding a new mapping to ensure that the same mapping is not added multiple times.
    /// </summary>
    /// <param name="mapping">The BankTransactionMapping entity to check for duplicates.</param>
    /// <returns>A task representing the asynchronous operation, containing a boolean value indicating whether the mapping is a duplicate.</returns>
    public async Task<bool> IsBankTransactionMappingDuplicateAsync(BankTransactionMapping mapping) =>    
        await _dbContext.BankTransactionMappings.AnyAsync(m =>
            m.SearchTerm == mapping.SearchTerm &&
            m.DescriptionTemplate == mapping.DescriptionTemplate &&
            m.ReferenceTemplate == mapping.ReferenceTemplate &&
            m.TargetGeneralLedgerAccountId == mapping.TargetGeneralLedgerAccountId);
    

}

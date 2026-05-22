using LedgerPro.Core.Entities;
using LedgerPro.Core.Exceptions;
using LedgerPro.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using LedgerPro.Application.DTOs.Reports;
using System.Data.Common;
using LedgerPro.Application.Extensions;
using LedgerPro.Application.DTOs.BankStatement;
using LedgerPro.Core.Enums;

namespace LedgerPro.Infrastructure.Repositories;

public class BankTransactionRepository(LedgerDbContext dbContext) : IBankTransactionRepository
{
    private readonly LedgerDbContext _dbContext = dbContext;

    /// <summary>
    /// Retrieves a BankTransaction entity by its unique identifier (ID). This method is used to fetch a specific bank transaction from the database, 
    /// typically for reconciliation or detailed viewing purposes. If the transaction with the specified ID does not exist, a KeyNotFoundException is 
    /// thrown to indicate that the requested transaction could not be found in the database.
    /// </summary>
    /// <param name="bankTransactionId">The unique identifier of the bank transaction.</param>
    /// <returns>The BankTransaction entity with the specified ID.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the bank transaction with the specified ID is not found.</exception>
    public async Task<BankTransaction> GetBankTransactionByIdAsync(Guid bankTransactionId)
    {
        return await _dbContext.BankTransactions.FindAsync(bankTransactionId) 
            ?? throw new KeyNotFoundException($"Bank transaction with ID {bankTransactionId} not found.");
    }

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
    
    /// <summary>
    /// Retrieves a list of BankTransactionRowDto objects for a specific BankSourceId. 
    /// This method is used to get the transaction data in a format suitable for display in the UI,
    /// including details such as transaction date, description, amount, type, status, and associated general ledger accounts.
    /// </summary>
    /// <param name="bankSourceId">The ID of the bank source for which to retrieve transaction rows.</param>
    /// <param name="financialYearEnding">The ending year of the financial year to filter transactions.</param>
    /// <returns>A list of BankTransactionRowDto objects.</returns>
    public async Task<List<BankTransactionRowDto>> GetBankTransactionRowsAsync(Guid bankSourceId, int? financialYearEnding) 
    {
        // Validate that the year provided is within a reasonable range (e.g., 1900 to 2100) to prevent invalid queries
        if (financialYearEnding != null && (financialYearEnding < 1900 || financialYearEnding > 2100))
        {
            throw new ArgumentOutOfRangeException(nameof(financialYearEnding), "Financial year ending must be between 1900 and 2100.");
        }

        financialYearEnding ??= DateTime.Now.GetFinancialYearEnding().Year;

        var bankTransactions = await _dbContext.BankTransactions
            .Where(t => t.BankSourceId == bankSourceId)
            .Select(t => new
            {
                t.Id,
                t.TransactionDate,
                t.Description,
                t.Amount,
                t.TransactionType,
                t.Status,
                GeneralLedgerAccounts = t.GeneralLedgerItems.Select(item => item.GeneralLedgerAccount.Name).ToList()   
            })
            .ToListAsync();

        return bankTransactions.Select(t => new BankTransactionRowDto(
            t.Id,
            t.TransactionDate,
            t.Description,
            t.Amount,
            t.TransactionType,
            t.Status,
            string.Join(", ", t.GeneralLedgerAccounts) // Concatenate account names into a single string
        )).ToList();
    }

    /// <summary>
    /// Reconciles a bank transaction by adding the associated general ledger items and updating the transaction status to Reconciled.
    /// This method takes a BankTransaction entity and a list of GeneralLedgerItem entities to add to the transaction. 
    /// It validates the input parameters, updates the bank transaction status, and adds the general ledger items to the database context.
    /// </summary>
    /// <param name="bankTransaction">The bank transaction to reconcile.</param>
    /// <param name="generalLedgerItemsToAdd">The list of general ledger items to add to the bank transaction.</param>
    /// <returns>The number of general ledger items added.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the bank transaction is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the list of general ledger items is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the bank transaction has already been reconciled or if the total amount of the general ledger items does not match the bank transaction amount.</exception>
    public async Task<int> ReconcileBankTransactionAsync(BankTransaction bankTransaction, List<GeneralLedgerItem> generalLedgerItemsToAdd)
    {
        if (bankTransaction == null)
            throw new ArgumentNullException(nameof(bankTransaction), "The bank transaction cannot be null.");

        if (bankTransaction.Status == BankTransactionStatus.Reconciled)
            throw new InvalidOperationException("The bank transaction has already been reconciled.");

        if (generalLedgerItemsToAdd == null || generalLedgerItemsToAdd.Count == 0)
            throw new ArgumentException("At least one general ledger item is required for reconciliation.", nameof(generalLedgerItemsToAdd));

        // Ensure that the total amount of the general ledger items matches the amount of the bank transaction to maintain data integrity.
        // Do not include bank transaction items to calculate the total correctly.
        if (generalLedgerItemsToAdd.Where(i => i.GeneralLedgerAccountId > 1010).Sum(i => i.Amount) != bankTransaction.Amount)
            throw new InvalidOperationException("The total amount of the general ledger items must equal the amount of the bank transaction.");
        
        // Set as reconciled.
        bankTransaction.Status = BankTransactionStatus.Reconciled;        
        // Add the general ledger items to the bank transaction's collection of general ledger items.
        ((List<GeneralLedgerItem>)bankTransaction.GeneralLedgerItems).AddRange(generalLedgerItemsToAdd);
        // Update the bank transaction.
        _dbContext.BankTransactions.Update(bankTransaction);
        
        return generalLedgerItemsToAdd.Count();
    }

    /// <summary>
    /// Unreconciles a bank transaction by removing all associated general ledger items and updating the transaction status to Pending.
    /// This method takes a BankTransaction entity to unreconcile. It validates the input parameter, checks that the transaction 
    /// is currently reconciled, updates the transaction status, and removes all associated general ledger items from the database context.
    /// </summary>
    /// <param name="bankTransaction">The bank transaction to unreconcile.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the bank transaction is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the bank transaction is not reconciled.</exception>
    public async Task UnreconcileBankTransactionAsync(BankTransaction bankTransaction)
    {
        if (bankTransaction == null)
            throw new ArgumentNullException(nameof(bankTransaction), "The bank transaction cannot be null.");

        if (bankTransaction.Status != BankTransactionStatus.Reconciled)
            throw new InvalidOperationException("Only reconciled bank transactions can be unreconciled.");

        // Set as unreconciled.
        bankTransaction.Status = BankTransactionStatus.Pending;        

        // Remove all associated general ledger items from the bank transaction.        
        _dbContext.GeneralLedgerItems.RemoveRange(bankTransaction.GeneralLedgerItems);
        
        // Update the bank transaction.
        _dbContext.BankTransactions.Update(bankTransaction);
    }

    /// <summary>
    /// Confirms the reconciliation of a categorized bank transaction by updating the associated general ledger item to set IsReconciled to true.
    /// </summary>
    /// <param name="bankTransaction">The bank transaction to confirm reconciliation for.</param>
    /// <param name="bankTransactionGlItem">The general ledger item associated with the bank transaction.</param>
    /// <returns>A task representing the asynchronous operation, with the result being the number of affected rows.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the bank transaction or general ledger item is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the bank transaction is not categorized.</exception>
    public async Task ConfirmReconcileCategorizedBankTransactionAsync(BankTransaction bankTransaction, GeneralLedgerItem bankTransactionGlItem)
    {
        if (bankTransaction == null)
            throw new ArgumentNullException(nameof(bankTransaction), "The bank transaction cannot be null.");

        if (bankTransactionGlItem == null)
            throw new ArgumentNullException(nameof(bankTransactionGlItem), "The bank transaction general ledger item cannot be null.");

        if (bankTransaction.Status != BankTransactionStatus.Categorized)
            throw new InvalidOperationException("Only categorized bank transactions can be confirmed for reconciliation.");

        // Update the bank transaction general ledger item to set IsReconciled to true.
        bankTransactionGlItem.IsReconciled = true;
        // As a reconciled bank transaction, add the bank transaction general ledger item.
        _dbContext.GeneralLedgerItems.Add(bankTransactionGlItem);
    }
}

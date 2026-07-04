using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.DTOs.Reports;
using Microsoft.EntityFrameworkCore;
using LedgerPro.Infrastructure.Extensions;
using LedgerPro.Application.DTOs.GeneralLedgerItem;

namespace LedgerPro.Infrastructure.Repositories;

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
    /// Retrieves a list of GeneralLedgerItemTransaction DTOs that are associated with a specific bank transaction ID.
    /// This method is used to fetch the ledger items that belong to a particular bank transaction.
    /// </summary>
    /// <param name="bankTransactionId">The ID of the bank transaction.</param>
    /// <returns>A list of GeneralLedgerItemTransaction DTOs.</returns>
    public async Task<List<GeneralLedgerItemTransaction>> GetGeneralLedgerItemsForBankTransactionAsync(Guid bankTransactionId) =>
        await _dbContext.GeneralLedgerItems
            .Where(item => item.BankTransactionId == bankTransactionId && item.GeneralLedgerAccountId > 1010) // Exclude bank-side ledger items
            .Select(item => new GeneralLedgerItemTransaction
            {
                Description = item.Description,
                Reference = item.Reference,
                Amount = item.Amount,
                GeneralLedgerAccountName = item.GeneralLedgerAccount.Name
            })
            .ToListAsync();

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
    /// Removes a collection of GeneralLedgerItem entities from the database context. This method is used to delete existing
    /// GeneralLedgerItems, for example, when correcting errors or unreconciled transactions. 
    /// It ensures that the specified ledger items are marked for deletion in the context, and they will be removed 
    /// from the database when SaveChangesAsync is called.
    /// </summary>
    /// <param name="ledgerItems">The collection of GeneralLedgerItem entities to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteGeneralLedgerItemsAsync(IEnumerable<GeneralLedgerItem> ledgerItems) =>
        _dbContext.GeneralLedgerItems.RemoveRange(ledgerItems);

    /// <summary>
    /// Retrieves all GeneralLedgerAccount entities from the database. This method is used to get the list of available 
    /// general ledger accounts.
    /// </summary>
    /// <returns>A list of GeneralLedgerAccount entities.</returns>
    public async Task<List<GeneralLedgerAccount>> GetGeneralLedgerAccountsAsync() =>
        await _dbContext.GeneralLedgerAccounts.ToListAsync();

    /// <summary>
    /// Retrieves a list of GeneralLedgerAccount entities whose IDs fall within the specified range. This method is used to
    /// filter general ledger accounts based on their IDs, which can be useful for reporting or selection purposes when only a subset of accounts is relevant.
    /// </summary>
    /// <param name="startAccountId">The starting ID of the range.</param>
    /// <param name="endAccountId">The ending ID of the range.</param>
    /// <returns>A list of GeneralLedgerAccount entities within the specified ID range.</returns>
    public async Task<List<GeneralLedgerAccount>> GetGeneralLedgerAccountsByRangeAsync(int startAccountId, int endAccountId) =>
        await _dbContext.GeneralLedgerAccounts
            .Where(account => account.Id >= startAccountId && account.Id <= endAccountId)
            .ToListAsync();

    /// <summary>
    /// Adds a new GeneralLedgerAccount entity to the database context. 
    /// This method is used to create a new general ledger account that can be associated with GeneralLedgerItems for categorization and reporting purposes.
    /// </summary>
    /// <param name="glAccount">The GeneralLedgerAccount entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddGeneralLedgerAccountAsync(GeneralLedgerAccount glAccount) =>
        await _dbContext.GeneralLedgerAccounts.AddAsync(glAccount);

    /// <summary>
    /// Updates an existing GeneralLedgerAccount entity in the database context. 
    /// This method is used to modify the details of an existing general ledger account, such as its name or account type. It first checks if the provided 
    /// GeneralLedgerAccount object is null and throws an ArgumentNullException if it is.
    /// </summary>
    /// <param name="id">The ID of the GeneralLedgerAccount to update.</param>
    /// <param name="glAccount">The GeneralLedgerAccount entity with updated values.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided GeneralLedgerAccount is null.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the GeneralLedgerAccount with the specified ID is not found.</exception>
    public async Task UpdateGeneralLedgerAccountAsync(int id, GeneralLedgerAccount glAccount)
    {
        if (glAccount == null)
            throw new ArgumentNullException(nameof(glAccount), "GeneralLedgerAccount cannot be null.");

        var existingAccount = await _dbContext.GeneralLedgerAccounts.FindAsync(id) ??
            throw new KeyNotFoundException($"GeneralLedgerAccount with ID {id} not found.");        

        _dbContext.Entry(existingAccount).UpdateFrom(glAccount);
    }

    /// <summary>
    /// Deletes an existing GeneralLedgerAccount entity from the database context. 
    /// This method is used to remove a general ledger account that is no longer needed.
    /// </summary>
    /// <param name="id">The ID of the GeneralLedgerAccount to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the GeneralLedgerAccount with the specified ID is not found.</exception>
    public async Task DeleteGeneralLedgerAccountAsync(int id)
    {
        var accountToDelete = await _dbContext.GeneralLedgerAccounts.FindAsync(id) ??
            throw new KeyNotFoundException($"GeneralLedgerAccount with ID {id} not found.");

        _dbContext.GeneralLedgerAccounts.Remove(accountToDelete);
    }

    /// <summary>
    /// Checks if a given general ledger account ID is currently in use by any GeneralLedgerItem entities or BankTransactionMappings. 
    /// This method is used to prevent the deletion of general ledger accounts that are still referenced by existing ledger items or bank transaction mappings, ensuring data integrity.
    /// </summary>
    /// <param name="accountId">The ID of the general ledger account to check.</param>
    /// <returns>A task representing the asynchronous operation, containing a boolean value indicating whether the account is in use.</returns>
    public async Task<bool> IsGeneralLedgerAccountIdInUseAsync(int accountId)
    {
        bool isInUseInLedgerItems = await _dbContext.GeneralLedgerItems.AnyAsync(item => item.GeneralLedgerAccountId == accountId);
        bool isInUseInBankTransactionMappings = await _dbContext.BankTransactionMappings.AnyAsync(mapping => mapping.TargetGeneralLedgerAccountId == accountId);

        return isInUseInLedgerItems || isInUseInBankTransactionMappings;
    }

    /// <summary>
    /// Retrieves financial totals (total debits and total credits) for each general ledger account within a specified date range.
    /// </summary>
    /// <param name="startDate">The start date of the period for which to calculate totals.</param>
    /// <param name="endDate">The end date of the period for which to calculate totals.</param>
    /// <returns>A list of financial totals for each general ledger account.</returns>
    public async Task<List<GlAccountFinancialTotal>> GetGlAccountFinancialTotalAsync(DateTime startDate, DateTime endDate)    
    {
        return await _dbContext.GeneralLedgerAccounts
            .Where(account => account.Id > 1010) // Exclude bank transaction accounts
            .Select(account => new GlAccountFinancialTotal
            (
                account.Id,
                account.Name,
                account.AccountType,
                account.GeneralLedgerItems
                    .Where(item => item.TransactionDate >= startDate && item.TransactionDate <= endDate && item.Side == TransactionSide.Debit)
                    .Sum(item => item.Amount),
                account.GeneralLedgerItems
                    .Where(item => item.TransactionDate >= startDate && item.TransactionDate <= endDate && item.Side == TransactionSide.Credit)
                    .Sum(item => item.Amount)
            ))
            .ToListAsync();
    } 

    /// <summary>
    /// Retrieves a list of GeneralLedgerItemSummaryTotal entities that represent the total amounts for each general ledger account type within a specified date range, 
    /// filtered by a provided account type mapping. This method is used to gather the relevant ledger items for generating a dashboard summary, allowing for filtering 
    /// based on account types to include only those that are relevant for the summary metrics (e.g., income, expenses, assets, liabilities).
    /// </summary>
    /// <param name="startDate">The start date of the period for which to retrieve ledger items.</param>
    /// <param name="endDate">The end date of the period for which to retrieve ledger items.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of GeneralLedgerItemLight entities.</returns>
    public async Task<List<GeneralLedgerItemLight>> GetMonthlyTotalsForDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbContext.GeneralLedgerItems
            .Where(item => item.TransactionDate >= startDate && item.TransactionDate <= endDate &&
                (item.GeneralLedgerAccount.AccountType == GeneralLedgerAccountType.Revenue || 
                item.GeneralLedgerAccount.AccountType == GeneralLedgerAccountType.Expense || 
                item.GeneralLedgerAccount.AccountType == GeneralLedgerAccountType.Liability))
            .Select(item => new GeneralLedgerItemLight(
                item.Amount,
                item.TransactionDate,
                item.Side,
                item.GeneralLedgerAccount.AccountType))
            .ToListAsync();
    } 

    /// <summary>
    /// Retrieves a list of GeneralLedgerItem entities that fall within a specified date range and match certain account type criteria.
    /// This method is used to gather the relevant ledger items for generating a dashboard summary, allowing for filtering based on account 
    /// types to include only those that are relevant for the summary metrics (e.g., income, expenses, assets, liabilities).
    /// </summary>
    /// <param name="fromDate">The start date of the period for which to retrieve ledger items.</param>
    /// <param name="toDate">The end date of the period for which to retrieve ledger items.</param>
    /// <param name="accountTypeMapping">A dictionary mapping account types to include in the summary.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of GeneralLedgerItemSummaryTotal entities.</returns>
    public async Task<List<GeneralLedgerItemSummaryTotal>> GetDashboardSummaryGeneralLedgerItemsAsync(DateTime fromDate, DateTime toDate, Dictionary<GeneralLedgerAccountType, GeneralLedgerAccountType> accountTypeMapping)
    {     
        // We filter the GeneralLedgerItems based on the transaction date and whether their associated account type is included in the provided mapping. 
        // Additionally, we exclude accounts with IDs 1010 and below, as these are typically reserved for the bank transactions.        
        return  await _dbContext.GeneralLedgerItems
            .Where(item => 
                item.TransactionDate >= fromDate && 
                item.TransactionDate <= toDate && 
                accountTypeMapping.ContainsKey(item.GeneralLedgerAccount.AccountType) &&
                item.GeneralLedgerAccountId > 1010)
            .Select(item => new GeneralLedgerItemSummaryTotal(
                item.GeneralLedgerAccount.AccountType,
                item.Amount,
                item.Side))
            .ToListAsync();        
    }

    /// <summary>
    /// Retrieves the count of unreconciled transactions within a specified date range. 
    /// This method is used to determine how many transactions have not yet been reconciled, which can be an important metric for financial management and reporting.
    /// </summary>
    /// <param name="fromDate">The start date of the period for which to count unreconciled transactions.</param>
    /// <param name="toDate">The end date of the period for which to count unreconciled transactions.</param>
    /// <returns>A task representing the asynchronous operation, containing the count of unreconciled transactions.</returns>
    public async Task<int> GetUnreconciledTransactionsCountAsync(DateTime fromDate, DateTime toDate)
    {
        return await _dbContext.GeneralLedgerItems
            .Where(item => 
                item.TransactionDate >= fromDate && 
                item.TransactionDate <= toDate && 
                !item.IsReconciled)
            .CountAsync();
    }

    /// <summary>
    /// Retrieves the count of GeneralLedgerItem entities that are associated with a specific bank transaction ID. 
    /// This method is used to check if there are any ledger items linked to a particular bank transaction, which can be useful for determining if a 
    /// transaction has already been categorized or if there are any existing records that reference it before performing operations such as deletion or categorization.
    /// Also for generating unique reference numbers when not provided by the user during categorization, ensuring that each ledger item can be traced back to its 
    /// source transaction.
    /// </summary>
    /// <param name="bankTransactionId">The ID of the bank transaction for which to count associated ledger items.</param>
    /// <returns>A task representing the asynchronous operation, containing the count of ledger items associated with the specified bank transaction.</returns>
    public async Task<int> GetGeneralLedgerItemCountForBankTransactionAsync(Guid bankTransactionId) =>
        await _dbContext.GeneralLedgerItems
            .Where(item => item.BankTransactionId == bankTransactionId)
            .CountAsync();
    
}

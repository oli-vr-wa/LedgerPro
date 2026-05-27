using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.DTOs.Reports;
using Microsoft.EntityFrameworkCore;

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
    /// Checks if a given general ledger account ID is currently in use by any GeneralLedgerItem entities. 
    /// This method is used to prevent the deletion of general ledger accounts that are still referenced by existing ledger items, ensuring data integrity.
    /// </summary>
    /// <param name="accountId">The ID of the general ledger account to check.</param>
    /// <returns>A task representing the asynchronous operation, containing a boolean value indicating whether the account is in use.</returns>
    public async Task<bool> IsGeneralLedgerAccountIdInUseAsync(int accountId)
    {
        return await _dbContext.GeneralLedgerItems.AnyAsync(item => item.GeneralLedgerAccountId == accountId);
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
}

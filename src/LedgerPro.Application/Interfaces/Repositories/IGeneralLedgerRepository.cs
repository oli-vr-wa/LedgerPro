using LedgerPro.Core.Entities;
using LedgerPro.Application.DTOs.Reports;
using LedgerPro.Core.Enums;
using LedgerPro.Application.DTOs.GeneralLedgerItem;
using LedgerPro.Application.DTOs.GeneralLedger;

namespace LedgerPro.Application.Interfaces.Repositories;

public interface IGeneralLedgerRepository
{
    Task<List<GeneralLedgerItem>> GetGeneralLedgerItemsAsync();
    Task<List<GeneralLedgerItemLight>> GetMonthlyTotalsForDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<List<GeneralLedgerItemTransaction>> GetGeneralLedgerItemsForBankTransactionAsync(Guid bankTransactionId);
    Task<List<GeneralLedgerItem>> GetGeneralLedgerItemsForRangeAndAccountAsync(DateTime startDate, DateTime endDate, int accountId);
    Task AddGeneralLedgerItemsAsync(IEnumerable<GeneralLedgerItem> ledgerItems);
    Task DeleteGeneralLedgerItemsAsync(IEnumerable<GeneralLedgerItem> ledgerItems);
    Task<GeneralLedgerAccount?> GetAccountByIdAsync(int accountId);
    Task<List<GeneralLedgerAccount>> GetGeneralLedgerAccountsAsync();
    Task<List<GeneralLedgerAccount>> GetGeneralLedgerAccountsByRangeAsync(int startAccountId, int endAccountId);
    Task AddGeneralLedgerAccountAsync(GeneralLedgerAccount glAccount);
    Task UpdateGeneralLedgerAccountAsync(int id, GeneralLedgerAccount glAccount);
    Task DeleteGeneralLedgerAccountAsync(int id);
    Task<bool> IsGeneralLedgerAccountIdInUseAsync(int accountId);
    Task<List<GeneralLedgerFinancialYearRow>> GetGeneralLedgerFinancialYearRowsAsync();
    Task<List<GlAccountFinancialTotal>> GetGlAccountFinancialTotalAsync(DateTime startDate, DateTime endDate);
    Task<List<GeneralLedgerItemSummaryTotal>> GetDashboardSummaryGeneralLedgerItemsAsync(DateTime fromDate, DateTime toDate, Dictionary<GeneralLedgerAccountType, GeneralLedgerAccountType> accountTypeMapping);
    Task<int> GetUnreconciledTransactionsCountAsync(DateTime fromDate, DateTime toDate);
    Task<int> GetGeneralLedgerItemCountForBankTransactionAsync(Guid bankTransactionId);
}
using LedgerPro.Core.Entities;
using LedgerPro.Application.DTOs.Reports;
using LedgerPro.Core.Enums;

namespace LedgerPro.Application.Interfaces.Repositories;

public interface IGeneralLedgerRepository
{
    Task<List<GeneralLedgerItem>> GetGeneralLedgerItemsAsync();
    Task<List<GeneralLedgerItemLight>> GetMonthlyTotalsForDateRangeAsync(DateTime startDate, DateTime endDate);
    Task AddGeneralLedgerItemsAsync(IEnumerable<GeneralLedgerItem> ledgerItems);
    Task DeleteGeneralLedgerItemsAsync(IEnumerable<GeneralLedgerItem> ledgerItems);
    Task<List<GeneralLedgerAccount>> GetGeneralLedgerAccountsAsync();
    Task<List<GeneralLedgerAccount>> GetGeneralLedgerAccountsByRangeAsync(int startAccountId, int endAccountId);
    Task AddGeneralLedgerAccountAsync(GeneralLedgerAccount glAccount);
    Task UpdateGeneralLedgerAccountAsync(int id, GeneralLedgerAccount glAccount);
    Task DeleteGeneralLedgerAccountAsync(int id);
    Task<bool> IsGeneralLedgerAccountIdInUseAsync(int accountId);
    Task<List<GlAccountFinancialTotal>> GetGlAccountFinancialTotalAsync(DateTime startDate, DateTime endDate);
    Task<List<GeneralLedgerItemSummaryTotal>> GetDashboardSummaryGeneralLedgerItemsAsync(DateTime fromDate, DateTime toDate, Dictionary<GeneralLedgerAccountType, GeneralLedgerAccountType> accountTypeMapping);
    Task<int> GetUnreconciledTransactionsCountAsync(DateTime fromDate, DateTime toDate);
}
using LedgerPro.Core.Entities;
using LedgerPro.Application.DTOs.Reports;

namespace LedgerPro.Application.Interfaces.Repositories;

public interface IGeneralLedgerRepository
{
    Task<List<GeneralLedgerItem>> GetGeneralLedgerItemsAsync();
    Task AddGeneralLedgerItemsAsync(IEnumerable<GeneralLedgerItem> ledgerItems);
    Task<List<GeneralLedgerAccount>> GetGeneralLedgerAccountsAsync();
    Task AddGeneralLedgerAccountAsync(GeneralLedgerAccount glAccount);
    Task<bool> IsGeneralLedgerAccountIdInUseAsync(int accountId);
    Task<List<GlAccountFinancialTotal>> GetGlAccountFinancialTotalAsync(DateTime startDate, DateTime endDate);
}
using LedgerPro.Core.Entities;

namespace LedgerPro.Core.Interfaces;

public interface IGeneralLedgerService
{
    Task AddGeneralLedgerAccountAsync(GeneralLedgerAccount account); 
    Task<List<IAccountSummaryRowDto>> GetFinancialYearAccountsSummaryAsync(int? financialYearEnding = null);  
}

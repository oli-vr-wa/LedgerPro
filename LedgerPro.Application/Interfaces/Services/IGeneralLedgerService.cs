using LedgerPro.Application.DTOs.Reports;
using LedgerPro.Core.Entities;

namespace LedgerPro.Application.Interfaces.Services;

public interface IGeneralLedgerService
{
    Task AddGeneralLedgerAccountAsync(GeneralLedgerAccount account); 
    Task<List<AccountSummaryRowDto>> GetFinancialYearAccountsSummaryAsync(int? financialYearEnding = null);  
}

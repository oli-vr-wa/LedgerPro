using LedgerPro.Application.DTOs.BankStatement;
using LedgerPro.Application.DTOs.Reports;
using LedgerPro.Core.Common;
using LedgerPro.Core.Entities;

namespace LedgerPro.Application.Interfaces.Services;

public interface IGeneralLedgerService
{
    Task AddGeneralLedgerAccountAsync(GeneralLedgerAccount account); 
    Task DeleteGeneralLedgerAccountAsync(int id);
    Task<List<AccountSummaryRowDto>> GetFinancialYearAccountsSummaryAsync(int? financialYearEnding = null);  
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(int financialYearEnding);
    Task<PeriodTotalsDto> GetMonthlyTotalsForDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Result<bool>> CreateGeneralLedgerItemsAsync(BankTransactionCategorize categorizeDto);
    GeneralLedgerItem CreateGeneralLedgerItem(BankTransaction bankTransaction, BankTransactionCategorizeItem categorizeDto);  
    Task<string> GenerateGeneralLedgerItemReferenceAsync(Guid bankTransactionId, DateTime transactionDate);
}

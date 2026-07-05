using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LedgerPro.Application.DTOs.Reports;

namespace LedgerPro.Application.Interfaces.Services;

public interface IGeneralLedgerReportService
{
    /// <summary>
    /// Retrieves a list of GeneralLedgerItemRowBalanceDto objects for a specific financial year and account ID.
    /// This method is to be used for generating the double-entry general ledger report.
    /// </summary>
    /// <param name="financialYear">The financial year for which to retrieve the general ledger items.</param>
    /// <param name="accountId">The ID of the account for which to retrieve the general ledger items.</param>
    /// <returns>A list of GeneralLedgerItemRowBalanceDto objects.</returns>
    Task<List<GeneralLedgerItemRowBalanceDto>> GetGeneralLedgerItemsForFinancialYearAndAccountAsync(int financialYear, int accountId);
}

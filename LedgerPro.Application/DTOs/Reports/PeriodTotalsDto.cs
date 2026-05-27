using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LedgerPro.Application.DTOs.Reports;

public record PeriodTotalsDto(
    List<MonthlyTotalsDto> MonthlyTotals, 
    int TotalPendingReconcileCount);


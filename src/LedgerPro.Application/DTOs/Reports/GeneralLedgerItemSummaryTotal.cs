using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LedgerPro.Core.Enums;

namespace LedgerPro.Application.DTOs.Reports;

public record GeneralLedgerItemSummaryTotal(
    GeneralLedgerAccountType AccountType,
    decimal Amount,
    TransactionSide Side);

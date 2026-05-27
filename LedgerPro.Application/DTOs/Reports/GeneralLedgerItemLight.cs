using LedgerPro.Core.Enums;

namespace LedgerPro.Application.DTOs.Reports;

public record GeneralLedgerItemLight(
    decimal Amount,
    DateTime TransactionDate,
    TransactionSide Side,
    GeneralLedgerAccountType AccountType
);

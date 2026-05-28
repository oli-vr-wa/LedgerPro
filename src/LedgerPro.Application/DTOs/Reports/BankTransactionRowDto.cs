using LedgerPro.Core.Enums;

namespace LedgerPro.Application.DTOs.Reports;

public record BankTransactionRowDto(
    Guid Id,
    DateTime TransactionDate,
    string Description,
    decimal Amount,
    string TransactionType,
    BankTransactionStatus Status,
    string GeneralLedgerClassification
);

    


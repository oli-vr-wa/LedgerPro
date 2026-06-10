namespace LedgerPro.Application.DTOs.BankStatement;

public record StatementImportRow
(
    string FileName,
    DateTime ImportDate,
    string BankAccountName,
    int TransactionCount,
    DateTime FirstTransactionDate,
    DateTime LastTransactionDate
);
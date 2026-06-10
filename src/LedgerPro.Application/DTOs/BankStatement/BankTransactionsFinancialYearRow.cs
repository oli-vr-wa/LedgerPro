namespace LedgerPro.Application.DTOs.BankStatement;

/// <summary>
/// Represents a summary row for bank transactions in a financial year. 
/// Contains the year ending, the date of the last transaction, and the count of pending transactions for that year.
/// This DTO is used to provide an overview of the bank transactions for each financial year, which can be useful for reporting and analysis.
/// </summary>
/// <param name="YearEnding">The year ending for the financial year.</param>
/// <param name="LastTransactionDate">The date of the last transaction in the financial year.</param>
/// <param name="PendingCount">The count of pending transactions for the financial year.</param>
public record BankTransactionsFinancialYearRow(
    int YearEnding,
    DateTime? LastTransactionDate,
    int PendingCount
);

namespace LedgerPro.Application.DTOs.BankStatement;

/// <summary>
/// Represents a row of bank source transactions data, used for displaying the latest transaction information for each bank source.
/// This record contains the bank source identifier, name, associated bank account name, and the dates of the last import and last transaction. 
/// It is designed to provide a quick overview of the most recent activity for each bank source, which can be useful for dashboards or summary views in the application.
/// The LastImportDate indicates when the last bank statement was imported for the bank source, while the LastTransactionDate shows the date of the most recent 
/// transaction imported from that source. This information helps users quickly identify which bank sources have recent activity and when they were last updated.
/// </summary>
/// <param name="BankSourceId">The unique identifier of the bank source.</param>
/// <param name="BankSourceName">The name of the bank source.</param>
/// <param name="BankAccountName">The name of the associated bank account.</param>
/// <param name="LastImportDate">The date when the last bank statement was imported for the bank source.</param>
/// <param name="LastTransactionDate">The date of the most recent transaction imported from the bank source.</param>
public record BankSourceTransactionsRow(
    Guid BankSourceId,
    string BankSourceName,
    string BankAccountName,
    DateTime? LastImportDate,
    DateTime? LastTransactionDate
);

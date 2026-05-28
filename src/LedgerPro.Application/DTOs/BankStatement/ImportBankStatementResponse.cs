namespace LedgerPro.Application.DTOs.BankStatement;

/// <summary>
/// DTO representing the response from importing a bank statement, containing a message and the count of transactions imported.
/// </summary>
/// <param name="Message">The message describing the result of the import operation.</param>
/// <param name="Count">The number of transactions successfully imported.</param>
public record ImportBankStatementResponse(
    string Message, 
    int Count
);

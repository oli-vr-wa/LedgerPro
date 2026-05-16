namespace LedgerPro.Application.DTOs.BankStatement;

/// <summary>
/// DTO representing the request to upload a bank statement, containing the bank source ID, the file stream of the bank statement, and the file name.
/// </summary>
/// <param name="BankSourceId">The ID of the bank source associated with the bank statement.</param>
/// <param name="FileStream">The stream of the bank statement file to be uploaded.</param>
/// <param name="FileName">The name of the bank statement file.</param>
public record UploadBankStatementRequest(
    Guid BankSourceId,
    Stream FileStream,
    string FileName
);

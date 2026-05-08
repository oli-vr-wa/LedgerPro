
namespace LedgerPro.Application.DTOs
{
    public record UploadBankStatementRequest(
        Guid BankSourceId,
        Stream FileStream,
        string FileName
    );
}
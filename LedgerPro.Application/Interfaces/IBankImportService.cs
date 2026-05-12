using LedgerPro.Application.DTOs.BankStatement;
using LedgerPro.Core.Common;

namespace LedgerPro.Application.Interfaces;

public interface IBankImportService
{
    Task<Result<int>> ImportBankStatementAsync(UploadBankStatementRequest request);
}

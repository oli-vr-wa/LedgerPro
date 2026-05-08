using LedgerPro.Application.DTOs;
using LedgerPro.Core.Common;

namespace LedgerPro.Application.Interfaces
{
    public interface IBankImportService
    {
        Task<Result<int>> ImportBankStatementAsync(UploadBankStatementRequest request);
    }
}
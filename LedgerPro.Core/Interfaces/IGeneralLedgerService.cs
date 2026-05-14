using LedgerPro.Core.Entities;
using LedgerPro.Core.Common;

namespace LedgerPro.Core.Interfaces;

public interface IGeneralLedgerService
{
    Task<Result<GeneralLedgerAccount>> AddGeneralLedgerAccountAsync(GeneralLedgerAccount account);
}

using LedgerPro.Application.DTOs.BankSource;

namespace LedgerPro.Application.Interfaces.Services;

public interface IBankSourceService
{
    Task<Guid> AddBankSourceAsync(AddBankSourceRequest request);
}

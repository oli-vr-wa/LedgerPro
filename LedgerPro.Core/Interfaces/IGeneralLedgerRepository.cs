using LedgerPro.Core.Entities;

namespace LedgerPro.Core.Interfaces
{
    public interface IGeneralLedgerRepository
    {
        Task<List<GeneralLedgerItem>> GetGeneralLedgerItemsAsync();
        Task AddGeneralLedgerItemsAsync(IEnumerable<GeneralLedgerItem> ledgerItems);
        Task AddGeneralLedgerAccountAsync(GeneralLedgerAccount glAccount);
    }
}

using LedgerPro.Core.Entities;

namespace LedgerPro.Core.Interfaces
{
    public interface ITransactionMatchService
    {
        GeneralLedgerItem? MatchAndCreateLedgerItem(BankTransaction bankTransaction, IEnumerable<BankTransactionMapping> mappings);
    }
}
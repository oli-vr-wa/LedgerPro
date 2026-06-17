using LedgerPro.Core.Entities;

namespace LedgerPro.Core.Interfaces;

public interface ITransactionMatchService
{
    GeneralLedgerItem? MatchAndCreateLedgerItem(BankTransaction bankTransaction, IEnumerable<BankTransactionMapping> mappings);
    IEnumerable<GeneralLedgerItem> MatchAndCreateLedgerItems(IEnumerable<BankTransaction> bankTransactions, IEnumerable<BankTransactionMapping> mappings);
}
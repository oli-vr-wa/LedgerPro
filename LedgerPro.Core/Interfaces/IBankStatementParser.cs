using LedgerPro.Core.Common;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;

namespace LedgerPro.Core.Interfaces
{
    public interface IBankStatementParser
    {
        // Pass a Stream to be handled by the API, and the ID of the BankSource to link the transactions to
        Result<IEnumerable<BankTransaction>> Parse(Stream fileStream, Guid bankSourceId, BankType bankType);
    }
}
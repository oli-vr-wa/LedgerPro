using CsvHelper.Configuration;
using LedgerPro.Core.Entities;

namespace LedgerPro.Infrastructure.Parsers;

public class NabStatementMap : ClassMap<BankTransaction>
{
    public NabStatementMap(Guid bankSourceId)
    {
        Map(m => m.TransactionDate).Name("Date");
        Map(m => m.Amount).Name("Amount");
        Map(m => m.Description).Name("Transaction Details");
        Map(m => m.TransactionType).Name("Transaction Type");

        // Set the BankSourceId for all records
        Map(m => m.BankSourceId).Constant(bankSourceId);

        // Ensure the ID is generated as a new GUID for each record
        Map(m => m.Id).Convert(args => Guid.NewGuid());
    }
}

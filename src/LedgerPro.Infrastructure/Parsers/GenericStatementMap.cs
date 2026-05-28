using CsvHelper.Configuration;
using LedgerPro.Core.Entities;

namespace LedgerPro.Infrastructure.Parsers;

public class GenericStatementMap : ClassMap<BankTransaction>
{
    public GenericStatementMap(Guid bankSourceId)
    {
        Map(m => m.TransactionDate).Name("Date");
        Map(m => m.Amount).Name("Amount");
        Map(m => m.Description).Name("Description");
        Map(m => m.TransactionType).Name("Transaction Type");

        Map(m => m.BankSourceId).Constant(bankSourceId);
        Map(m => m.Id).Convert(_ => Guid.NewGuid()); // Generate a new GUID for each transaction
    }
}

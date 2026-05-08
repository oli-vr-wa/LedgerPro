using System.Globalization;
using CsvHelper;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Interfaces;

namespace LedgerPro.Infrastructure.Parsers
{
    public class BankStatementParser : IBankStatementParser
    {
        public IEnumerable<BankTransaction> Parse(Stream fileStream, Guid bankSourceId)
        {
            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // Register the mapping for NAB statements
            csv.Context.RegisterClassMap(new NabStatementMap(bankSourceId));

            return csv.GetRecords<BankTransaction>().ToList();
        }
    }
}
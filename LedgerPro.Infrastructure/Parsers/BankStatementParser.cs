using System.Globalization;
using CsvHelper;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using LedgerPro.Core.Interfaces;

namespace LedgerPro.Infrastructure.Parsers
{
    public class BankStatementParser : IBankStatementParser
    {
        public IEnumerable<BankTransaction> Parse(Stream fileStream, Guid bankSourceId, BankType bankType)
        {
            using var reader = new StreamReader(fileStream);

            var culture = new CultureInfo("en-AU"); // Set culture to Australian English for date parsing
            using var csv = new CsvReader(reader, culture);


            // Register the appropriate mapping based on the bank type
            switch (bankType)
            {
                case BankType.NAB:
                    csv.Context.RegisterClassMap(new NabStatementMap(bankSourceId));
                    break;
                default:
                    csv.Context.RegisterClassMap(new GenericStatementMap(bankSourceId));
                    break;
            }

            return csv.GetRecords<BankTransaction>().ToList();
        }
    }
}
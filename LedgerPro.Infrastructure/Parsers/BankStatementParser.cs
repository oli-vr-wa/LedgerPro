using System.Globalization;
using CsvHelper;
using CsvHelper.TypeConversion;
using LedgerPro.Core.Common;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using LedgerPro.Core.Interfaces;

namespace LedgerPro.Infrastructure.Parsers
{
    public class BankStatementParser : IBankStatementParser
    {
        /// <summary>
        /// Parses a bank statement from a given Stream, associating the transactions with a specified BankSource ID and handling different bank formats 
        /// based on the BankType enum. Returns a Result object containing either a list of BankTransaction entities or an error message if parsing fails.
        /// </summary>
        /// <param name="fileStream">The stream containing the bank statement data.</param>
        /// <param name="bankSourceId">The ID of the bank source to associate the transactions with.</param>
        /// <param name="bankType">The type of the bank for which to parse the statement.</param>
        /// <returns>A Result object containing the parsed transactions or an error message.</returns>
        public Result<IEnumerable<BankTransaction>> Parse(Stream fileStream, Guid bankSourceId, BankType bankType)
        {
            try
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

                var records = csv.GetRecords<BankTransaction>().ToList();
                return Result<IEnumerable<BankTransaction>>.Success(records);
            }
            catch (HeaderValidationException ex)
            {
                return Result<IEnumerable<BankTransaction>>.Failure($"CSV header validation mismatch: {ex.Message}");
            }
            catch (TypeConverterException ex)
            {
                return Result<IEnumerable<BankTransaction>>.Failure($"Data Format Error: Could not parse value {ex.Text} at row {ex.Context?.Parser?.Row ?? -1}.");
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<BankTransaction>>.Failure($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
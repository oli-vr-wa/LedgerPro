using Xunit;
using LedgerPro.Infrastructure.Parsers;
using System.Text;

namespace LedgerPro.Tests
{
    public class NabParserTests
    {
        [Fact]
        public void Parse_ValidNabCsv_ReturnsBankTransactions()
        {
            // Arrange
            var csvContent = "Date,Amount,Account Number,,Transaction Type,Transaction Details,Balance\n" +
                                "01 Jan 24,-102.53,123456789,,EFTPOS DEBIT,COLES 1234,1000.00\n" +
                                "02 Jan 24,2500.00,123456789,,DIRECT CREDIT,Wages,3500.00\n";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            var parser = new BankStatementParser();
            var bankSourceId = Guid.NewGuid();

            // Act
            var result = parser.Parse(stream, bankSourceId);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal(new DateTime(2024, 1, 1), result.First().TransactionDate);
            Assert.Equal(new DateTime(2024, 1, 2), result.Skip(1).First().TransactionDate);
            Assert.Equal(-102.53m, result.First().Amount);
            Assert.Equal(2500.00m, result.Skip(1).First().Amount);
            Assert.Equal("COLES 1234", result.First().Description);
            Assert.Equal("Wages", result.Skip(1).First().Description);
            Assert.Equal("EFTPOS DEBIT", result.First().TransactionType);
            Assert.Equal("DIRECT CREDIT", result.Skip(1).First().TransactionType);
        }
    }
}
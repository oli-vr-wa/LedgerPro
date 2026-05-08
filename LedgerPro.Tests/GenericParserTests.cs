using LedgerPro.Infrastructure.Parsers;
using LedgerPro.Core.Enums;
using System.Text;

namespace LedgerPro.Tests
{
    public class GenericParserTests
    {
        [Fact]
        public void Parse_ValidGenericCsv_ReturnsBankTransactions()
        {
            // Arrange
            var csvContent = "Date,Amount,Description,Transaction Type\n" +
                                "01/01/2024,-102.53,COLES 1234,EFTPOS DEBIT\n" +
                                "02/01/2024,2500.00,Wages,DIRECT CREDIT\n";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            var parser = new BankStatementParser();
            var bankSourceId = Guid.NewGuid();

            // Act
            var result = parser.Parse(stream, bankSourceId, BankType.Generic);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
            Assert.Equal(new DateTime(2024, 1, 1), result.Value.First().TransactionDate);
            Assert.Equal(new DateTime(2024, 1, 2), result.Value.Skip(1).First().TransactionDate);
            Assert.Equal(-102.53m, result.Value.First().Amount);
            Assert.Equal(2500.00m, result.Value.Skip(1).First().Amount);
            Assert.Equal("COLES 1234", result.Value.First().Description);
            Assert.Equal("Wages", result.Value.Skip(1).First().Description);
            Assert.Equal("EFTPOS DEBIT", result.Value.First().TransactionType);
            Assert.Equal("DIRECT CREDIT", result.Value.Skip(1).First().TransactionType);
        }
    }
}
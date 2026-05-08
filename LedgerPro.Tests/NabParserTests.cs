using Xunit;
using LedgerPro.Infrastructure.Parsers;
using LedgerPro.Core.Enums;
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
            var result = parser.Parse(stream, bankSourceId, BankType.NAB);

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

        [Fact]
        public void Parse_InvalidNabCsv_ReturnsFailure()
        {
            // Arrange
            var csvContent = "Date,Amount,Account Number,,Transaction Type,Transaction Details,Balance\n" +
                                "Invalid Date,-102.53,123456789,,EFTPOS DEBIT,COLES 1234,1000.00\n";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            var parser = new BankStatementParser();
            var bankSourceId = Guid.NewGuid();

            // Act
            var result = parser.Parse(stream, bankSourceId, BankType.NAB);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Data Format Error", result.Error);            
        }

        [Fact]
        public void Parse_MissingHeaders_ReturnsFailure()
        {
            // Arrange
            var csvContent = "Date,Amount,Account Number,Transaction Details,Balance\n" +  // Missing the "Transaction Type" header which is required for NAB parsing
                                "01 Jan 24,-102.53,123456789,COLES 1234,1000.00\n";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

            var parser = new BankStatementParser();
            var bankSourceId = Guid.NewGuid();

            // Act
            var result = parser.Parse(stream, bankSourceId, BankType.NAB);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("CSV header validation mismatch", result.Error);
        }
    }
}
using LedgerPro.Core.Interfaces;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using LedgerPro.Core.Common;
using LedgerPro.Application.Services;
using NSubstitute;
using LedgerPro.Application.DTOs;

namespace LedgerPro.Tests
{
    public class BankImportServiceTests
    {
        private readonly IBankStatementParser _bankStatementParser = Substitute.For<IBankStatementParser>();
        private readonly IBankRepository _bankRepository = Substitute.For<IBankRepository>();
        private readonly ITransactionMatchService _transactionMatchService = Substitute.For<ITransactionMatchService>();
        private readonly IFileHasher _fileHasher = Substitute.For<IFileHasher>();
        private readonly BankImportService _bankImportService;

        public BankImportServiceTests()
        {
            _bankImportService = new BankImportService(_bankStatementParser, _transactionMatchService, _bankRepository, _fileHasher);
        }

        /// <summary>
        /// Tests that when the bank source does not exist, the service returns a failure result with the appropriate error message.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ImportBankStatement_WhenSourceDoesNotExist_ReturnsFailure()
        {
            // Arrange
            var request = new UploadBankStatementRequest(Guid.NewGuid(), Stream.Null, "test.csv");

            // Set the mock to return null for the bank source
            _bankRepository.GetBankSourceByIdAsync(request.BankSourceId).Returns((BankSource)null!);

            // Act
            var result = await _bankImportService.ImportBankStatementAsync(request);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal($"Bank source with ID {request.BankSourceId} not found.", result.Error);
        }

        /// <summary>
        /// Tests that when the bank statement parser fails, the service returns a failure result with the appropriate error message.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ImportBankStatement_WhenParserFails_ReturnsFailure()
        {
            // Arrange
            var bankSourceId = Guid.NewGuid();
            var request = new UploadBankStatementRequest(bankSourceId, Stream.Null, "test.csv");

            // Mock the file hasher to return a specific hash value
            _fileHasher.CalculateHashAsync(request.FileStream).Returns("hashedvalue");

            // Set the mock to return a valid bank source
            _bankRepository.GetBankSourceByIdAsync(request.BankSourceId).Returns(new BankSource { Id = bankSourceId, BankType = BankType.Generic });

            // Set the parser to return a failure result
            _bankStatementParser.Parse(request.FileStream, request.BankSourceId, BankType.Generic).Returns(Result<IEnumerable<BankTransaction>>.Failure("Parsing error"));

            // Act
            var result = await _bankImportService.ImportBankStatementAsync(request);

            // Assert
            Assert.True(result.IsFailure);
            Assert.Equal("Parsing error", result.Error);
        }

        /// <summary>
        /// Tests that when the bank statement parser succeeds, the service returns a success result with the parsed transactions.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ImportBankStatement_WhenParserSucceeds_ReturnsSuccess()
        {
            // Arrange
            var bankSourceId = Guid.NewGuid();
            var request = new UploadBankStatementRequest(bankSourceId, Stream.Null, "test.csv");

            // Set the mock to return a valid bank source
            _bankRepository.GetBankSourceByIdAsync(request.BankSourceId).Returns(new BankSource { Id = bankSourceId, BankType = BankType.Generic });

            // Mock the file hasher to return a specific hash value
            _fileHasher.CalculateHashAsync(request.FileStream).Returns("hashedvalue");

            // Set the parser to return a successful result with some transactions
            var transactions = new List<BankTransaction>
            {
                new BankTransaction { Id = Guid.NewGuid(), Amount = 100, Description = "Test Transaction 1", BankSourceId = bankSourceId },
                new BankTransaction { Id = Guid.NewGuid(), Amount = -50, Description = "Test Transaction 2", BankSourceId = bankSourceId }
            };

            _bankStatementParser.Parse(request.FileStream, request.BankSourceId, BankType.Generic).Returns(Result<IEnumerable<BankTransaction>>.Success(transactions));

            // Set the SaveChangesAsync to return the number of transactions added
            _bankRepository.SaveChangesAsync().Returns(transactions.Count);

            // Act
            var result = await _bankImportService.ImportBankStatementAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(transactions.Count, result.Value);

            // Verify that the transactions were added to the database context
            await _bankRepository.Received(1).AddTransactionsAsync(Arg.Is<IEnumerable<BankTransaction>>(t => t.Count() == transactions.Count()));;

            // Verify that SaveChangesAsync was called
            await _bankRepository.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task ImportBankStatement_WhenTransactionsMatchMappings_CreatesGeneralLedgerItems()
        {
            // Arrange
            var bankSourceId = Guid.NewGuid();
            var request = new UploadBankStatementRequest(bankSourceId, Stream.Null, "test.csv");

            // Set the mock to return a valid bank source
            _bankRepository.GetBankSourceByIdAsync(request.BankSourceId).Returns(new BankSource { Id = bankSourceId, BankType = BankType.Generic });

            // Mock the file hasher to return a specific hash value
            _fileHasher.CalculateHashAsync(request.FileStream).Returns("hashedvalue");

            // Set the parser to return a successful result with some transactions
            var transactions = new List<BankTransaction>
            {
                new BankTransaction { Id = Guid.NewGuid(), Amount = 100, Description = "Woolworths B12345", BankSourceId = bankSourceId }
            };

            _bankStatementParser.Parse(request.FileStream, request.BankSourceId, BankType.Generic).Returns(Result<IEnumerable<BankTransaction>>.Success(transactions));

            // Set up some mappings that will match the transactions
            var mappings = new List<BankTransactionMapping>
            {
                new BankTransactionMapping 
                { 
                    Id = Guid.NewGuid(), 
                    SearchTerm = "Woolworths", 
                    MatchStrategy = BankTransactionMatchStrategy.Contains, 
                    TargetGeneralLedgerAccountId = 5000,
                    ReferenceTemplate = "GROC-WOOLIES",
                    DescriptionTemplate = "Grocery purchase - Woolworths", 
                    Priority = 1
                },
                new BankTransactionMapping 
                { 
                    Id = Guid.NewGuid(), 
                    SearchTerm = "Coles", 
                    MatchStrategy = BankTransactionMatchStrategy.Contains, 
                    TargetGeneralLedgerAccountId = 5001,
                    ReferenceTemplate = "GROC-COLES",
                    DescriptionTemplate = "Grocery purchase - Coles", 
                    Priority = 1
                }
            };   
            // Set the repository to return the mappings
            _bankRepository.GetBankTransactionMappingsAsync().Returns(mappings);       
            
            // Set the transaction match service to return a GeneralLedgerItem when a match is found
            _transactionMatchService.MatchAndCreateLedgerItem(Arg.Is<BankTransaction>(t => t.Description.Contains("Woolworths")), mappings)
                .Returns(new GeneralLedgerItem { Id = Guid.NewGuid(), Description = "Grocery purchase - Woolworths", Amount = 100, GeneralLedgerAccountId = 5000 });

            // Act
            var result = await _bankImportService.ImportBankStatementAsync(request);

            // Assert
            Assert.True(result.IsSuccess);
            
            // Verify that GeneralLedgerItems were created for the matched transactions
            await _bankRepository
                .Received(1)
                .AddGLItemsAsync(Arg.Is<IEnumerable<GeneralLedgerItem>>(gli => gli.Any(item => item.Description.Contains("Grocery purchase - Woolworths"))));
        }
    }
}
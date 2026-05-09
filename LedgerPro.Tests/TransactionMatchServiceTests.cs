using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using LedgerPro.Core.Services;

namespace LedgerPro.Tests
{
    public class TransactionMatchServiceTests
    {
        [Fact]
        public void MatchAndCreateGeneralLedgerItem_WhenTransactionMatchesMapping_ReturnsBankTransaction()
        {
            // Arrange
            var transactionMatchService = new TransactionMatchService();
            Guid bankSourceId = Guid.NewGuid();
            Guid statementImportId = Guid.NewGuid();

            var bankTransaction = new BankTransaction
            {
                Id = Guid.NewGuid(),
                Description = "Woolworths* Q1234567",
                Amount = -134.56m,
                TransactionDate = new DateTime(2024, 1, 1),
                BankSourceId = bankSourceId,
                StatementImportId = statementImportId,
                TransactionType = "EFTPOS DEBIT",
                Status = BankTransactionStatus.Pending
            };

            var mappingRulesList = new List<BankTransactionMapping>
            {
                new BankTransactionMapping
                {
                    Id = Guid.NewGuid(),
                    SearchTerm = "Woolworths",
                    MatchStrategy = BankTransactionMatchStrategy.Contains,
                    TargetGeneralLedgerAccountId = 5000, // Example account ID for Groceries
                    ReferenceTemplate = "GROCERY-WOOLIES",
                    DescriptionTemplate = "Grocery Expense - Woolworths",
                    Priority = 1
                }
            };

            // Act
            var result = transactionMatchService.MatchAndCreateLedgerItem(bankTransaction, mappingRulesList);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bankTransaction.TransactionDate, result!.TransactionDate);
            Assert.Equal("GROCERY-WOOLIES", result.Reference);
            Assert.Equal("Grocery Expense - Woolworths", result.Description);
            Assert.Equal(134.56m, result.Amount); // Amount should be positive in the ledger item
            Assert.Equal(bankTransaction.Id, result.BankTransactionId);
            Assert.Equal(5000, result.GeneralLedgerAccountId);
            Assert.Equal(TransactionSide.Debit, result.Side);
            Assert.Equal(BankTransactionStatus.Categorized, bankTransaction.Status); // Ensure the transaction status is updated to Categorized            
        }

        [Fact]
        public void MatchAndCreateGeneralLedgerItem_WhenTransactionDoesNotMatchMapping_ReturnsNull()
        {
            // Arrange
            var transactionMatchService = new TransactionMatchService();
            Guid bankSourceId = Guid.NewGuid();
            Guid statementImportId = Guid.NewGuid();

            var bankTransaction = new BankTransaction
            {
                Id = Guid.NewGuid(),
                Description = "Coles Supermarket Q1234567",
                Amount = -50.00m,
                TransactionDate = new DateTime(2024, 1, 2),
                BankSourceId = bankSourceId,
                StatementImportId = statementImportId,
                TransactionType = "EFTPOS DEBIT",
                Status = BankTransactionStatus.Pending
            };

            var mappingRulesList = new List<BankTransactionMapping>
            {
                new BankTransactionMapping
                {
                    Id = Guid.NewGuid(),
                    SearchTerm = "Woolworths",
                    MatchStrategy = BankTransactionMatchStrategy.Contains,
                    TargetGeneralLedgerAccountId = 5000, // Example account ID for Groceries
                    ReferenceTemplate = "GROCERY-WOOLIES",
                    DescriptionTemplate = "Grocery Expense - Woolworths",
                    Priority = 1
                }
            };

            // Act
            var result = transactionMatchService.MatchAndCreateLedgerItem(bankTransaction, mappingRulesList);

            // Assert
            Assert.Null(result);
            Assert.Equal(BankTransactionStatus.Pending, bankTransaction.Status); // Ensure the transaction status remains unchanged
        }

        [Fact]
        public void MatchAndCreateGeneralLederItem_WhenMatchUsingRegexMapping_ReturnsBankTransaction()
        {
            // Arrange
            var transactionMatchService = new TransactionMatchService();
            Guid bankSourceId = Guid.NewGuid();
            Guid statementImportId = Guid.NewGuid();

            var bankTransaction = new BankTransaction
            {
                Id = Guid.NewGuid(),
                Description = "*UBER TRIP 123456",
                Amount = -20.00m,
                TransactionDate = new DateTime(2024, 1, 3),
                BankSourceId = bankSourceId,
                StatementImportId = statementImportId,
                TransactionType = "EFTPOS DEBIT",
                Status = BankTransactionStatus.Pending
            };

            var mappingRulesList = new List<BankTransactionMapping>
            {
                new BankTransactionMapping
                {
                    Id = Guid.NewGuid(),
                    SearchTerm = @"\*UBER TRIP \d+",
                    MatchStrategy = BankTransactionMatchStrategy.Regex,
                    TargetGeneralLedgerAccountId = 6000, // Example account ID for Transportation
                    ReferenceTemplate = "TRANSPORT-UBER",
                    DescriptionTemplate = "Travel Expense - Uber",
                    Priority = 1
                }
            };

            // Act
            var result = transactionMatchService.MatchAndCreateLedgerItem(bankTransaction, mappingRulesList);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bankTransaction.TransactionDate, result!.TransactionDate);
            Assert.Equal("TRANSPORT-UBER", result.Reference);
            Assert.Equal("Travel Expense - Uber", result.Description);
            Assert.Equal(20.00m, result.Amount); // Amount should be positive in the ledger item
            Assert.Equal(bankTransaction.Id, result.BankTransactionId);
            Assert.Equal(6000, result.GeneralLedgerAccountId);
            Assert.Equal(TransactionSide.Debit, result.Side);
            Assert.Equal(BankTransactionStatus.Categorized, bankTransaction.Status); // Ensure the transaction status is updated to Categorized

        }

        [Fact]
        public void MatchAndCreateGeneralLedgerItem_WhenMultipleMappingsMatch_ReturnsHighestPriorityMapping()
        {
            // Arrange
            var transactionMatchService = new TransactionMatchService();
            Guid bankSourceId = Guid.NewGuid();
            Guid statementImportId = Guid.NewGuid();

            var bankTransaction = new BankTransaction
            {
                Id = Guid.NewGuid(),
                Description = "Woolworths Supermarket Q1234567",
                Amount = -150.00m,
                TransactionDate = new DateTime(2024, 1, 4),
                BankSourceId = bankSourceId,
                StatementImportId = statementImportId,
                TransactionType = "EFTPOS DEBIT",
                Status = BankTransactionStatus.Pending
            };

            var mappingRulesList = new List<BankTransactionMapping>
            {
                new BankTransactionMapping
                {
                    Id = Guid.NewGuid(),
                    SearchTerm = "Woolworths",
                    MatchStrategy = BankTransactionMatchStrategy.Contains,
                    TargetGeneralLedgerAccountId = 5000, // Example account ID for Groceries
                    ReferenceTemplate = "GROCERY-WOOLIES",
                    DescriptionTemplate = "Grocery Expense - Woolworths",
                    Priority = 1 // Higher priority (lower number)
                },
                new BankTransactionMapping
                {
                    Id = Guid.NewGuid(),
                    SearchTerm = "Supermarket",
                    MatchStrategy = BankTransactionMatchStrategy.Contains,
                    TargetGeneralLedgerAccountId = 5001, // Example account ID for General Expenses
                    ReferenceTemplate = "EXPENSE-SUPERMARKET",
                    DescriptionTemplate = "General Expense - Supermarket",
                    Priority = 2 
                }
            };

            // Act
            var result = transactionMatchService.MatchAndCreateLedgerItem(bankTransaction, mappingRulesList);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bankTransaction.TransactionDate, result!.TransactionDate);
            Assert.Equal("GROCERY-WOOLIES", result.Reference); // Should match the higher priority mapping
            Assert.Equal("Grocery Expense - Woolworths", result.Description);
            Assert.Equal(150.00m, result.Amount); // Amount should be positive in the ledger item
            Assert.Equal(bankTransaction.Id, result.BankTransactionId);
            Assert.Equal(5000, result.GeneralLedgerAccountId); // Should match the higher priority mapping's account ID
            Assert.Equal(TransactionSide.Debit, result.Side);
        }
    }
}
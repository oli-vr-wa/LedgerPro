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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Application.Services;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using LedgerPro.Core.Interfaces;
using NSubstitute;

namespace LedgerPro.Tests.Application.Services.BankTransactionServiceTests;

public class ConfirmReconcileCategorizedBankTransactionTests
{
    private readonly IBankTransactionRepository _bankTransactionRepository = Substitute.For<IBankTransactionRepository>();
    private readonly ITransactionMatchService _transactionMatchService = Substitute.For<ITransactionMatchService>();
    private readonly IGeneralLedgerRepository _generalLedgerRepository = Substitute.For<IGeneralLedgerRepository>();
    private readonly IBankTransactionService _bankTransactionService;
    

    public ConfirmReconcileCategorizedBankTransactionTests()
    {
        _bankTransactionService = new BankTransactionService(_bankTransactionRepository, _transactionMatchService, _generalLedgerRepository);
    }

    [Fact]
    public async Task ConfirmReconcileCategorizedBankTransactionAsync_ReturnsTrue_WhenReconciliationIsSuccessful()
    {
        // Arrange
        var bankTransactionId = Guid.NewGuid();
        var bankTransaction = new BankTransaction
        {
            Id = bankTransactionId,
            Amount = 100,
            Description = "Test Transaction",
            TransactionDate = DateTime.Now,
            Status = BankTransactionStatus.Categorized,
            BankSource = new BankSource
            {
                Id = Guid.NewGuid(),
                BankName = "Test Bank Source",
                GeneralLedgerAccountId = 1000
            },
            GeneralLedgerItems = new List<GeneralLedgerItem>()
            {
                new GeneralLedgerItem
                {
                    Id = Guid.NewGuid(),
                    GeneralLedgerAccountId = 2000,
                    Amount = 100,
                    Description = "Test GL Item",
                    TransactionDate = DateTime.Now,
                    Side = TransactionSide.Credit,
                    BankTransactionId = bankTransactionId
                }
            }
        };

        var expectedGlItem = new GeneralLedgerItem
        {
            GeneralLedgerAccountId = bankTransaction.BankSource.GeneralLedgerAccountId,
            Amount = bankTransaction.Amount,
            Description = $"{bankTransaction.Description} - Reconciled Bank Transaction",
            TransactionDate = bankTransaction.TransactionDate,
            IsReconciled = true,
            Side = bankTransaction.Amount < 0 ? TransactionSide.Debit : TransactionSide.Credit
        };

        _bankTransactionRepository.GetBankTransactionByIdAsync(bankTransactionId).Returns(bankTransaction);        
        _bankTransactionRepository.ConfirmReconcileCategorizedBankTransactionAsync(bankTransaction, Arg.Any<GeneralLedgerItem>()).Returns(Task.CompletedTask);

        // Act
        await _bankTransactionService.ConfirmReconcileCategorizedBankTransactionAsync(bankTransactionId);

        // Assert
        await _bankTransactionRepository.Received(1).ConfirmReconcileCategorizedBankTransactionAsync(bankTransaction, Arg.Any<GeneralLedgerItem>());
        // Verify that the general ledger item was created with the correct properties
        

        await _bankTransactionRepository.Received(1).ConfirmReconcileCategorizedBankTransactionAsync(bankTransaction, Arg.Is<GeneralLedgerItem>(glItem =>
            glItem.GeneralLedgerAccountId == expectedGlItem.GeneralLedgerAccountId &&
            glItem.Amount == expectedGlItem.Amount &&
            glItem.Description == expectedGlItem.Description &&
            glItem.TransactionDate == expectedGlItem.TransactionDate &&
            glItem.IsReconciled == expectedGlItem.IsReconciled &&
            glItem.Side == expectedGlItem.Side
        ));
    }

    [Fact]
    public async Task ConfirmReconcileCategorizedBankTransactionAsync_ThrowsArgumentException_WhenBankTransactionIdIsEmpty()
    {
        // Arrange
        var emptyBankTransactionId = Guid.Empty;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _bankTransactionService.ConfirmReconcileCategorizedBankTransactionAsync(emptyBankTransactionId));
        Assert.Equal("The bank transaction ID cannot be empty. (Parameter 'bankTransactionId')", exception.Message);
        // Verify that the repository method was not called
        await _bankTransactionRepository.DidNotReceiveWithAnyArgs().GetBankTransactionByIdAsync(Arg.Any<Guid>());
        await _bankTransactionRepository.DidNotReceiveWithAnyArgs().ConfirmReconcileCategorizedBankTransactionAsync(Arg.Any<BankTransaction>(), Arg.Any<GeneralLedgerItem>());
    }

    [Fact]
    public async Task ConfirmReconcileCategorizedBankTransactionAsync_ThrowsInvalidOperationException_WhenBankTransactionIsNotCategorized()
    {
        // Arrange
        var bankTransactionId = Guid.NewGuid();
        var bankTransaction = new BankTransaction
        {
            Id = bankTransactionId,
            Amount = 100,
            Description = "Test Transaction",
            TransactionDate = DateTime.Now,
            Status = BankTransactionStatus.Pending, // Not categorized
        };

        _bankTransactionRepository.GetBankTransactionByIdAsync(bankTransactionId).Returns(bankTransaction);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _bankTransactionService.ConfirmReconcileCategorizedBankTransactionAsync(bankTransactionId));
        Assert.Equal("Only categorized bank transactions can be confirmed for reconciliation.", exception.Message);
        // Verify that the repository method was called to get the bank transaction but not to confirm reconciliation
        await _bankTransactionRepository.Received(1).GetBankTransactionByIdAsync(bankTransactionId);
        await _bankTransactionRepository.DidNotReceiveWithAnyArgs().ConfirmReconcileCategorizedBankTransactionAsync(Arg.Any<BankTransaction>(), Arg.Any<GeneralLedgerItem>());
    }
}

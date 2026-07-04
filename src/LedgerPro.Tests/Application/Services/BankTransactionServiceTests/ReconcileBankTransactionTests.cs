using NSubstitute;
using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Services;
using LedgerPro.Application.DTOs.BankStatement;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using LedgerPro.Core.Interfaces;

namespace LedgerPro.Tests.Application.Services.BankTransactionServiceTests;

public class ReconcileBankTransactionTests
{
    private readonly IBankTransactionRepository _bankTransactionRepository = Substitute.For<IBankTransactionRepository>();
    private readonly ITransactionMatchService _transactionMatchService = Substitute.For<ITransactionMatchService>();
    private readonly IGeneralLedgerRepository _generalLedgerRepository = Substitute.For<IGeneralLedgerRepository>();
    private readonly BankTransactionService _bankTransactionService;

    public ReconcileBankTransactionTests()
    {
        _bankTransactionService = new BankTransactionService(_bankTransactionRepository, _transactionMatchService, _generalLedgerRepository);
    }

    [Fact]
    public async Task ReconcileBankTransactionAsync_ThrowsArgumentNullException_WhenEmptyGuidProvided()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _bankTransactionService.ReconcileBankTransactionAsync(Guid.Empty));        
        Assert.Equal("The bank transaction ID cannot be empty. (Parameter 'bankTransactionId')", exception.Message);

        // Verify that the repository's GetBankTransactionByIdAsync method was not called since the request is null
        await _bankTransactionRepository.DidNotReceive().GetBankTransactionByIdAsync(Arg.Any<Guid>());
    }  

    [Fact]
    public async Task ReconcileBankTransactionAsync_ThrowsInvalidOperationException_WhenBankTransactionNotFound()
    {
        // Arrange
        var bankTransactionId = Guid.NewGuid();

        _bankTransactionRepository.GetBankTransactionByIdAsync(bankTransactionId).Returns((BankTransaction?)null!);    

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _bankTransactionService.ReconcileBankTransactionAsync(bankTransactionId));
        Assert.Equal("The bank transaction to reconcile was not found.", exception.Message);

        // Verify that the repository's GetBankTransactionWithGlItemsByIdAsync method was called with the correct bank transaction ID
        await _bankTransactionRepository.Received(1).GetBankTransactionWithGlItemsByIdAsync(bankTransactionId);

        // Verify that the repository's ReconcileBankTransactionAsync method was not called since the bank transaction was not found
        await _bankTransactionRepository.DidNotReceive().ReconcileBankTransactionAsync(Arg.Any<BankTransaction>(), Arg.Any<GeneralLedgerItem>());
    }

    [Fact]
    public async Task ReconcileBankTransactionAsync_ThrowsInvalidOperationException_WhenBankTransactionAlreadyReconciled()
    {
        // Arrange
        var bankTransactionId = Guid.NewGuid();
        var bankTransaction = new BankTransaction
        {
            Id = bankTransactionId,
            Amount = 700,
            Status = BankTransactionStatus.Reconciled
        };

        _bankTransactionRepository.GetBankTransactionWithGlItemsByIdAsync(bankTransactionId).Returns(bankTransaction);        

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _bankTransactionService.ReconcileBankTransactionAsync(bankTransactionId));
        Assert.Equal("The bank transaction has already been reconciled.", exception.Message);

        // Verify that the repository's GetBankTransactionWithGlItemsByIdAsync method was called with the correct bank transaction ID
        await _bankTransactionRepository.Received(1).GetBankTransactionWithGlItemsByIdAsync(bankTransactionId);

        // Verify that the repository's ReconcileBankTransactionAsync method was not called since the bank transaction was already reconciled
        await _bankTransactionRepository.DidNotReceive().ReconcileBankTransactionAsync(Arg.Any<BankTransaction>(), Arg.Any<GeneralLedgerItem>());
    }

    [Fact]
    public async Task ReconcileBankTransactionAsync_ThrowsInvalidOperationException_WhenSplitItemsTotalDoesNotMatchBankTransactionAmount()
    {
        // Arrange
        var bankTransactionId = Guid.NewGuid();
        var bankTransaction = new BankTransaction
        {
            Id = bankTransactionId,
            Amount = 1000,
            Status = BankTransactionStatus.Pending,
            GeneralLedgerItems = new List<GeneralLedgerItem>
            {
                new GeneralLedgerItem { Id = Guid.NewGuid(), Amount = 500, GeneralLedgerAccountId = 5000, Side = TransactionSide.Debit },
                new GeneralLedgerItem { Id = Guid.NewGuid(), Amount = 200, GeneralLedgerAccountId = 5001, Side = TransactionSide.Debit }
            }
        };

        _bankTransactionRepository.GetBankTransactionWithGlItemsByIdAsync(bankTransactionId).Returns(bankTransaction);        

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _bankTransactionService.ReconcileBankTransactionAsync(bankTransactionId));
        Assert.Equal("The total amount of the split general ledger items must equal the amount of the bank transaction.", exception.Message);

        // Verify that the repository's GetBankTransactionWithGlItemsByIdAsync method was called with the correct bank transaction ID
        await _bankTransactionRepository.Received(1).GetBankTransactionWithGlItemsByIdAsync(bankTransactionId);

        // Verify that the repository's ReconcileBankTransactionAsync method was not called since the split items total does not match the bank transaction amount
        await _bankTransactionRepository.DidNotReceive().ReconcileBankTransactionAsync(Arg.Any<BankTransaction>(), Arg.Any<GeneralLedgerItem>());
    }

    [Fact]
    public async Task ReconcileBankTransactionAsync_SuccessfullyReconcilesBankTransaction_WhenRequestIsValid()
    {
        // Arrange
        var bankTransactionId = Guid.NewGuid();
        var bankTransaction = new BankTransaction
        {
            Id = bankTransactionId,
            Amount = 1000,
            Status = BankTransactionStatus.Pending,
            TransactionDate = new DateTime(2024, 6, 1),
            BankSource = new BankSource
            {
                GeneralLedgerAccountId = 1001
            },
            GeneralLedgerItems = new List<GeneralLedgerItem>
            {
                new GeneralLedgerItem { Id = Guid.NewGuid(), Amount = 600, GeneralLedgerAccountId = 5000, Side = TransactionSide.Debit },
                new GeneralLedgerItem { Id = Guid.NewGuid(), Amount = 400, GeneralLedgerAccountId = 5001, Side = TransactionSide.Debit }
            }
        };

        _bankTransactionRepository.GetBankTransactionWithGlItemsByIdAsync(bankTransactionId).Returns(bankTransaction);        

        _bankTransactionRepository.ReconcileBankTransactionAsync(bankTransaction, Arg.Any<GeneralLedgerItem>()).Returns(true);

        // Act
        bool result = await _bankTransactionService.ReconcileBankTransactionAsync(bankTransactionId);

        // Assert
        Assert.True(result);

        // Verify that the repository's GetBankTransactionWithGlItemsByIdAsync method was called with the correct bank transaction ID
        await _bankTransactionRepository.Received(1).GetBankTransactionWithGlItemsByIdAsync(bankTransactionId);

        // Verify that the repository's ReconcileBankTransactionAsync method was called with the correct parameters
        await _bankTransactionRepository.Received(1).ReconcileBankTransactionAsync(
            Arg.Is<BankTransaction>(bt => bt.Id == bankTransactionId),
            Arg.Is<GeneralLedgerItem>(gli =>
                gli.BankTransactionId == bankTransactionId &&
                gli.GeneralLedgerAccountId == bankTransaction.BankSource.GeneralLedgerAccountId &&
                gli.IsReconciled)
        );
    }
}

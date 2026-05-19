using NSubstitute;
using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Services;
using LedgerPro.Application.DTOs.BankStatement;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;

namespace LedgerPro.Tests.Application.Services.BankTransactionServiceTests;

public class ReconcileBankTransactionTests
{
    private readonly IBankTransactionRepository _bankTransactionRepository = Substitute.For<IBankTransactionRepository>();
    private readonly BankTransactionService _bankTransactionService;

    public ReconcileBankTransactionTests()
    {
        _bankTransactionService = new BankTransactionService(_bankTransactionRepository);
    }

    [Fact]
    public async Task ReconcileBankTransactionAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _bankTransactionService.ReconcileBankTransactionAsync(null!));        
        Assert.Equal("The reconcile bank transaction request cannot be null. (Parameter 'request')", exception.Message);

        // Verify that the repository's GetBankTransactionByIdAsync method was not called since the request is null
        await _bankTransactionRepository.DidNotReceive().GetBankTransactionByIdAsync(Arg.Any<Guid>());
    }

    [Fact]
    public async Task ReconcileBankTransactionAsync_ThrowsArgumentException_WhenNoSplitItemsProvided()
    {
        // Arrange
        var request = new ReconcileBankTransactionRequest(
            Guid.NewGuid(),
            new List<SplitGeneralLedgerItemRequest>() // No split items provided
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _bankTransactionService.ReconcileBankTransactionAsync(request));
        Assert.Equal("At least one split general ledger item is required for reconciliation.", exception.Message);

        // Verify that the repository's GetBankTransactionByIdAsync method was not called since no split items are provided
        await _bankTransactionRepository.DidNotReceive().GetBankTransactionByIdAsync(Arg.Any<Guid>());
    }    

    [Fact]
    public async Task ReconcileBankTransactionAsync_ThrowsInvalidOperationException_WhenBankTransactionNotFound()
    {
        // Arrange
        var bankTransactionId = Guid.NewGuid();

        _bankTransactionRepository.GetBankTransactionByIdAsync(bankTransactionId).Returns((BankTransaction?)null!);
        
        var request = new ReconcileBankTransactionRequest(
            bankTransactionId,
            new List<SplitGeneralLedgerItemRequest>
            {
                new SplitGeneralLedgerItemRequest(5000, "GL Account - 1", "GLA-1", 500), // Total does not match bank transaction amount
                new SplitGeneralLedgerItemRequest(5001, "GL Account - 2", "GLA-2", 200)
            }
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _bankTransactionService.ReconcileBankTransactionAsync(request));
        Assert.Equal("The bank transaction to reconcile was not found.", exception.Message);

        // Verify that the repository's GetBankTransactionByIdAsync method was called with the correct bank transaction ID
        await _bankTransactionRepository.Received(1).GetBankTransactionByIdAsync(bankTransactionId);

        // Verify that the repository's ReconcileBankTransactionAsync method was not called since the bank transaction was not found
        await _bankTransactionRepository.DidNotReceive().ReconcileBankTransactionAsync(Arg.Any<BankTransaction>(), Arg.Any<List<GeneralLedgerItem>>());
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

        _bankTransactionRepository.GetBankTransactionByIdAsync(bankTransactionId).Returns(bankTransaction);
        
        var request = new ReconcileBankTransactionRequest(
            bankTransactionId,
            new List<SplitGeneralLedgerItemRequest>
            {
                new SplitGeneralLedgerItemRequest(5000, "GL Account - 1", "GLA-1", 500),
                new SplitGeneralLedgerItemRequest(5001, "GL Account - 2", "GLA-2", 200)
            }
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _bankTransactionService.ReconcileBankTransactionAsync(request));
        Assert.Equal("The bank transaction has already been reconciled.", exception.Message);

        // Verify that the repository's GetBankTransactionByIdAsync method was called with the correct bank transaction ID
        await _bankTransactionRepository.Received(1).GetBankTransactionByIdAsync(bankTransactionId);

        // Verify that the repository's ReconcileBankTransactionAsync method was not called since the bank transaction was already reconciled
        await _bankTransactionRepository.DidNotReceive().ReconcileBankTransactionAsync(Arg.Any<BankTransaction>(), Arg.Any<List<GeneralLedgerItem>>());
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
            Status = BankTransactionStatus.Pending
        };

        _bankTransactionRepository.GetBankTransactionByIdAsync(bankTransactionId).Returns(bankTransaction);
        
        var request = new ReconcileBankTransactionRequest(
            bankTransactionId,
            new List<SplitGeneralLedgerItemRequest>
            {
                new SplitGeneralLedgerItemRequest(5000, "GL Account - 1", "GLA-1", 500), // Total does not match bank transaction amount
                new SplitGeneralLedgerItemRequest(5001, "GL Account - 2", "GLA-2", 200)
            }
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _bankTransactionService.ReconcileBankTransactionAsync(request));
        Assert.Equal("The total amount of the split general ledger items must equal the amount of the bank transaction.", exception.Message);

        // Verify that the repository's GetBankTransactionByIdAsync method was called with the correct bank transaction ID
        await _bankTransactionRepository.Received(1).GetBankTransactionByIdAsync(bankTransactionId);

        // Verify that the repository's ReconcileBankTransactionAsync method was not called since the split items total does not match the bank transaction amount
        await _bankTransactionRepository.DidNotReceive().ReconcileBankTransactionAsync(Arg.Any<BankTransaction>(), Arg.Any<List<GeneralLedgerItem>>());
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
            TransactionDate = new DateTime(2024, 6, 1)
        };

        _bankTransactionRepository.GetBankTransactionByIdAsync(bankTransactionId).Returns(bankTransaction);
        
        var request = new ReconcileBankTransactionRequest(
            bankTransactionId,
            new List<SplitGeneralLedgerItemRequest>
            {
                new SplitGeneralLedgerItemRequest(5000, "GL Account - 1", "GLA-1", 600),
                new SplitGeneralLedgerItemRequest(5001, "GL Account - 2", "GLA-2", 400)
            }
        );

        _bankTransactionRepository.ReconcileBankTransactionAsync(bankTransaction, Arg.Any<List<GeneralLedgerItem>>()).Returns(2);

        // Act
        int glItemsAdded = await _bankTransactionService.ReconcileBankTransactionAsync(request);

        // Assert
        Assert.Equal(2, glItemsAdded);

        // Verify that the repository's GetBankTransactionByIdAsync method was called with the correct bank transaction ID
        await _bankTransactionRepository.Received(1).GetBankTransactionByIdAsync(bankTransactionId);

        // Verify that the repository's ReconcileBankTransactionAsync method was called with the correct parameters
        await _bankTransactionRepository.Received(1).ReconcileBankTransactionAsync(
            Arg.Is<BankTransaction>(bt => bt.Id == bankTransactionId),
            Arg.Is<List<GeneralLedgerItem>>(gli => gli.Count == 2 &&
                gli[0].GeneralLedgerAccountId == 5000 && gli[0].Amount == 600 &&
                gli[1].GeneralLedgerAccountId == 5001 && gli[1].Amount == 400)
        );
    }
}

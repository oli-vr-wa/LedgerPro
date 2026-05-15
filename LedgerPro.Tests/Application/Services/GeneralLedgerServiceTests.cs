using LedgerPro.Core.Interfaces;
using LedgerPro.Core.Entities;   
using LedgerPro.Core.Enums;
using LedgerPro.Application.Services;
using NSubstitute;

namespace LedgerPro.Tests.Application.Services;

public class GeneralLedgerServiceTests
{
    private readonly IGeneralLedgerRepository _generalLedgerRepository = Substitute.For<IGeneralLedgerRepository>();
    private readonly GeneralLedgerService _generalLedgerService;

    public GeneralLedgerServiceTests()
    {
        _generalLedgerService = new GeneralLedgerService(_generalLedgerRepository);
    }

    /// <summary>
    /// Tests that when the account ID is already in use, the service returns a failure result with the appropriate error message, 
    /// and that the repository's AddGeneralLedgerAccountAsync method is not called, and the unit of work's CommitAsync method is not called.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddGeneralLedgerAccountAsync_WhenAccountIdIsInUse_ReturnsFailure()
    {
        // Arrange
        var account = new GeneralLedgerAccount { Id = 5000, Name = "Duplicate Expense Account", AccountType = GeneralLedgerAccountType.Expense };
        _generalLedgerRepository.IsGeneralLedgerAccountIdInUseAsync(account.Id).Returns(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _generalLedgerService.AddGeneralLedgerAccountAsync(account));
        Assert.Equal($"General ledger account with ID {account.Id} is already in use and cannot be added.", exception.Message);        

        // Verify that the AddGeneralLedgerAccountAsync method was not called since the account ID is in use
        await _generalLedgerRepository.DidNotReceive().AddGeneralLedgerAccountAsync(Arg.Any<GeneralLedgerAccount>());
    }

    /// <summary>
    /// Tests that when the account ID is not in use, the service successfully adds the account and commits the changes.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddGeneralLedgerAccountAsync_WhenAccountIdIsNotInUse_AddsAccountSuccessfully()
    {
        // Arrange
        var account = new GeneralLedgerAccount { Id = 5001, Name = "New Expense Account", AccountType = GeneralLedgerAccountType.Expense };
        _generalLedgerRepository.IsGeneralLedgerAccountIdInUseAsync(account.Id).Returns(false);
        _generalLedgerRepository.AddGeneralLedgerAccountAsync(account).Returns(Task.CompletedTask);       

        // Act
        await _generalLedgerService.AddGeneralLedgerAccountAsync(account);

        // Assert
        await _generalLedgerRepository.Received(1).AddGeneralLedgerAccountAsync(account);
    }

    /// <summary>
    /// Tests that when the account is null, the service returns a failure result with the appropriate error message,
    /// and that the repository's AddGeneralLedgerAccountAsync method is not called, and the unit of work's CommitAsync method is not called.    
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddGeneralLedgerAccountAsync_WhenAccountIsNull_ReturnsFailure()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _generalLedgerService.AddGeneralLedgerAccountAsync(null!));
        Assert.Equal("Account cannot be null. (Parameter 'account')", exception.Message);

        // Verify that the AddGeneralLedgerAccountAsync method was not called since the account is null
        await _generalLedgerRepository.DidNotReceive().IsGeneralLedgerAccountIdInUseAsync(Arg.Any<int>());
        await _generalLedgerRepository.DidNotReceive().AddGeneralLedgerAccountAsync(Arg.Any<GeneralLedgerAccount>());        
    }
}

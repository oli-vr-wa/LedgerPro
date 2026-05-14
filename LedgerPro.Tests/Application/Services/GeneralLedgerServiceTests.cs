using LedgerPro.Core.Interfaces;
using LedgerPro.Core.Entities;   
using LedgerPro.Core.Enums;
using LedgerPro.Application.Services;
using NSubstitute;

namespace LedgerPro.Tests.Application.Services;

public class GeneralLedgerServiceTests
{
    private readonly IGeneralLedgerRepository _generalLedgerRepository = Substitute.For<IGeneralLedgerRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly GeneralLedgerService _generalLedgerService;

    public GeneralLedgerServiceTests()
    {
        _generalLedgerService = new GeneralLedgerService(_generalLedgerRepository, _unitOfWork);
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

        // Act
        var result = await _generalLedgerService.AddGeneralLedgerAccountAsync(account);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal($"General ledger account with ID {account.Id} is already in use and cannot be added.", result.Error);

        // Verify that the AddGeneralLedgerAccountAsync method was not called since the account ID is in use
        await _generalLedgerRepository.DidNotReceive().AddGeneralLedgerAccountAsync(Arg.Any<GeneralLedgerAccount>());
        await _unitOfWork.DidNotReceive().CommitAsync();
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
        _unitOfWork.CommitAsync().Returns(1);   // Simulate successful commit by returning 1 to indicate one change was saved

        // Act
        var result = await _generalLedgerService.AddGeneralLedgerAccountAsync(account);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(account, result.Value);

        // Verify that the AddGeneralLedgerAccountAsync method was called with the correct account and that CommitAsync was called
        await _generalLedgerRepository.Received(1).AddGeneralLedgerAccountAsync(account);
        await _unitOfWork.Received(1).CommitAsync();
    }

    /// <summary>
    /// Tests that when the account is null, the service returns a failure result with the appropriate error message,
    /// and that the repository's AddGeneralLedgerAccountAsync method is not called, and the unit of work's CommitAsync method is not called.    
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddGeneralLedgerAccountAsync_WhenAccountIsNull_ReturnsFailure()
    {
        // Act
        var result = await _generalLedgerService.AddGeneralLedgerAccountAsync(null!);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Account cannot be null.", result.Error);

        // Verify that the AddGeneralLedgerAccountAsync method was not called since the account is null
        await _generalLedgerRepository.DidNotReceive().AddGeneralLedgerAccountAsync(Arg.Any<GeneralLedgerAccount>());
        await _unitOfWork.DidNotReceive().CommitAsync();
    }
}

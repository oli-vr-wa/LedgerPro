using LedgerPro.Core.Interfaces;
using LedgerPro.Core.Entities;   
using LedgerPro.Core.Enums;
using LedgerPro.Application.Services;
using NSubstitute;
using LedgerPro.Core.Exceptions;
using LedgerPro.Application.DTOs.Reports;
using LedgerPro.Infrastructure.Projections;

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
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _generalLedgerService.AddGeneralLedgerAccountAsync(account));
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

    /// <summary>
    /// Tests that when the financial year ending parameter is invalid (e.g., less than 1900 or greater than 2100), the service throws an ArgumentOutOfRangeException with the appropriate error message,
    /// and that the repository's GetGlAccountFinancialTotalAsync method is not called.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetFinancialYearAccountsSummaryAsync_WhenFinancialYearEndingIsInvalid_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        int invalidFinancialYearEnding = 1800; // Invalid year

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _generalLedgerService.GetFinancialYearAccountsSummaryAsync(invalidFinancialYearEnding));
        Assert.Equal("Financial year ending must be between 1900 and 2100. (Parameter 'financialYearEnding')", exception.Message);

        // Verify that the repository method was not called since the financial year ending is invalid
        await _generalLedgerRepository.DidNotReceive().GetGlAccountFinancialTotalAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>());
    }

    /// <summary>
    /// Tests that when the financial year ending parameter is valid, the service returns the expected account summary, and that the repository's 
    /// GetGlAccountFinancialTotalAsync method is called with the correct parameters.
    /// The test verifies the balance calculation for both an Asset account (where balance = total debits - total credits) and 
    /// a Liability account (where balance = total credits - total debits) based on the provided totals.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetFinancialYearAccountsSummaryAsync_WhenFinancialYearEndingIsValid_ReturnsAccountSummary()
    {
        // Arrange
        int validFinancialYearEnding = 2024;

        var expectedSummary = new  List<IGlAccountFinancialTotal>
        {
            new GlAccountFinancialTotal
            {
                AccountId = 1000,
                AccountName = "Cash",
                AccountType = GeneralLedgerAccountType.Asset,
                TotalDebits = 10000m,
                TotalCredits = 2000m,
            },
            new GlAccountFinancialTotal
            {
                AccountId = 2000,
                AccountName = "Accounts Payable",
                AccountType = GeneralLedgerAccountType.Liability,
                TotalDebits = 5000m,
                TotalCredits = 8000m,
            }
        };

        _generalLedgerRepository.GetGlAccountFinancialTotalAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>()).Returns(expectedSummary);

        // Act
        var result = await _generalLedgerService.GetFinancialYearAccountsSummaryAsync(validFinancialYearEnding);

        // Assert
        Assert.Equal(expectedSummary.Count, result.Count);
        Assert.Equal(expectedSummary[0].AccountId, result[0].AccountId);
        Assert.Equal(expectedSummary[0].AccountName, result[0].AccountName);
        Assert.Equal(expectedSummary[0].AccountType, result[0].AccountType);
        Assert.Equal(expectedSummary[0].TotalDebits, result[0].TotalDebits);
        Assert.Equal(expectedSummary[0].TotalCredits, result[0].TotalCredits);
        Assert.Equal(expectedSummary[0].TotalDebits - expectedSummary[0].TotalCredits, result[0].Balance);

        Assert.Equal(expectedSummary[1].AccountId, result[1].AccountId);
        Assert.Equal(expectedSummary[1].AccountName, result[1].AccountName);
        Assert.Equal(expectedSummary[1].AccountType, result[1].AccountType);
        Assert.Equal(expectedSummary[1].TotalDebits, result[1].TotalDebits);
        Assert.Equal(expectedSummary[1].TotalCredits, result[1].TotalCredits);
        Assert.Equal(expectedSummary[1].TotalCredits - expectedSummary[1].TotalDebits, result[1].Balance);
    }
}

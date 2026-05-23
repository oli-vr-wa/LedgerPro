using LedgerPro.Application.DTOs.Reports;
using LedgerPro.Core.Enums;
using NSubstitute;

namespace LedgerPro.Tests.Application.Services.GeneralLedgerServiceTests;

public class GetFinancialYearAccountsSummaryTests : GeneralLedgerServiceTestsBase
{
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
        Assert.Contains("Invalid financial year", exception.Message);

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

        var expectedSummary = new  List<GlAccountFinancialTotal>
        {
            new GlAccountFinancialTotal(
                1000,
                "Cash",
                GeneralLedgerAccountType.Asset,
                10000m,
                2000m
            ),
            new GlAccountFinancialTotal(
                2000,
                "Accounts Payable",
                GeneralLedgerAccountType.Liability,
                5000m,
                8000m
            )
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

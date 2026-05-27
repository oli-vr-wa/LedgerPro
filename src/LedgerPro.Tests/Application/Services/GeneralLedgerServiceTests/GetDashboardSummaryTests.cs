using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LedgerPro.Application.DTOs.Reports;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using NSubstitute;

namespace LedgerPro.Tests.Application.Services.GeneralLedgerServiceTests;

public class GetDashboardSummaryTests : GeneralLedgerServiceTestsBase
{
    [Fact]
    public async Task GetDashboardSummaryAsync_WhenCalled_ReturnsDashboardSummary()
    {
        // Arrange
        int financialYearEnding = 2025;

        var generalLedgerItemSummaryTotals = new List<GeneralLedgerItemSummaryTotal>
        {
            new GeneralLedgerItemSummaryTotal(
                GeneralLedgerAccountType.Asset,
                1000m,
                TransactionSide.Debit
            ),
            new GeneralLedgerItemSummaryTotal(
                GeneralLedgerAccountType.Asset,
                500m,
                TransactionSide.Debit
            ),
            new GeneralLedgerItemSummaryTotal(
                GeneralLedgerAccountType.Liability,
                200m,
                TransactionSide.Credit
            ),
            new GeneralLedgerItemSummaryTotal(
                GeneralLedgerAccountType.Liability,
                50m,
                TransactionSide.Debit
            ),
            new GeneralLedgerItemSummaryTotal(
                GeneralLedgerAccountType.Revenue,
                300m,
                TransactionSide.Credit
            ),
            new GeneralLedgerItemSummaryTotal(
                GeneralLedgerAccountType.Expense,
                150m,
                TransactionSide.Debit
            ),
            new GeneralLedgerItemSummaryTotal(
                GeneralLedgerAccountType.Expense,
                100m,
                TransactionSide.Debit
            )
        };    

        int unreconciledTransactionsCount = 2;
        
        _generalLedgerRepository.GetDashboardSummaryGeneralLedgerItemsAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<Dictionary<GeneralLedgerAccountType, GeneralLedgerAccountType>>())
            .Returns(generalLedgerItemSummaryTotals);
        _generalLedgerRepository.GetUnreconciledTransactionsCountAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
            .Returns(unreconciledTransactionsCount);

        var expectedSummary = new DashboardSummaryDto
        {
            TotalIncome = 300m, 
            TotalExpense = 250m,
            Assets = 1500m, 
            Liabilities = 150m,
            UnreconciledTransactionsCount = unreconciledTransactionsCount
        };

        // Act
        var result = await _generalLedgerService.GetDashboardSummaryAsync(financialYearEnding);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedSummary.TotalIncome, result.TotalIncome);
        Assert.Equal(expectedSummary.TotalExpense, result.TotalExpense);
        Assert.Equal(expectedSummary.Assets, result.Assets);
        Assert.Equal(expectedSummary.Liabilities, result.Liabilities);
        Assert.Equal(expectedSummary.UnreconciledTransactionsCount, result.UnreconciledTransactionsCount);        
    }

    [Fact]
    public async Task GetDashboardSummaryAsync_WhenFinancialYearEndingIsInvalid_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        int invalidFinancialYearEnding = 1800; // Invalid year, outside the valid range

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _generalLedgerService.GetDashboardSummaryAsync(invalidFinancialYearEnding));
        Assert.Contains("Invalid financial year", exception.Message);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LedgerPro.Application.DTOs.Reports;
using LedgerPro.Core.Enums;
using NSubstitute;

namespace LedgerPro.Tests.Application.Services.GeneralLedgerServiceTests;

public class GetMonthlyTotalsForDateRangeTests : GeneralLedgerServiceTestsBase
{
    [Fact]
    public async Task GetMonthlyTotalsForDateRange_ReturnsCorrectTotals()
    {
        // Arrange
        var startDate = new DateTime(2024, 1, 1);
        var endDate = new DateTime(2024, 3, 31);
        int pendingReconcileCount = 2;

        var ledgerItems = new List<GeneralLedgerItemLight>
        {
            new GeneralLedgerItemLight(100, new DateTime(2024, 1, 15), TransactionSide.Credit, GeneralLedgerAccountType.Revenue),
            new GeneralLedgerItemLight(50, new DateTime(2024, 1, 20), TransactionSide.Debit, GeneralLedgerAccountType.Expense),
            new GeneralLedgerItemLight(200, new DateTime(2024, 2, 10), TransactionSide.Credit, GeneralLedgerAccountType.Revenue),
            new GeneralLedgerItemLight(75, new DateTime(2024, 2, 15), TransactionSide.Credit, GeneralLedgerAccountType.Liability),
            new GeneralLedgerItemLight(30, new DateTime(2024, 3, 5), TransactionSide.Debit, GeneralLedgerAccountType.Expense)
        };

        _generalLedgerRepository.GetMonthlyTotalsForDateRangeAsync(startDate, endDate).Returns(ledgerItems);
        _bankTransactionRepository.GetPendingReconciliationCountAsync(startDate, endDate).Returns(pendingReconcileCount);

        var expectedResults = new PeriodTotalsDto(
            new List<MonthlyTotalsDto>
            {
                new MonthlyTotalsDto { Year = 2024, Month = 1, TotalRevenue = 100, TotalExpense = 50, TotalLiability = 0 },
                new MonthlyTotalsDto { Year = 2024, Month = 2, TotalRevenue = 200, TotalExpense = 0, TotalLiability = 75 },
                new MonthlyTotalsDto { Year = 2024, Month = 3, TotalRevenue = 0, TotalExpense = 30, TotalLiability = 0 }
            },
            pendingReconcileCount
        );

        // Act
        var result = await _generalLedgerService.GetMonthlyTotalsForDateRangeAsync(startDate, endDate);

        // Assert
        Assert.Equal(3, result.MonthlyTotals.Count); // 3 months with transactions

        var januaryTotals = result.MonthlyTotals.FirstOrDefault(r => r.Year == 2024 && r.Month == 1);
        Assert.NotNull(januaryTotals);
        Assert.Equal(100, januaryTotals.TotalRevenue);
        Assert.Equal(50, januaryTotals.TotalExpense);
        Assert.Equal(0, januaryTotals.TotalLiability);
        Assert.Equal(pendingReconcileCount, result.TotalPendingReconcileCount);

        var februaryTotals = result.MonthlyTotals.FirstOrDefault(r => r.Year == 2024 && r.Month == 2);
        Assert.NotNull(februaryTotals);
        Assert.Equal(200, februaryTotals.TotalRevenue);
        Assert.Equal(0, februaryTotals.TotalExpense);
        Assert.Equal(75, februaryTotals.TotalLiability);

        var marchTotals = result.MonthlyTotals.FirstOrDefault(r => r.Year == 2024 && r.Month == 3);
        Assert.NotNull(marchTotals);
        Assert.Equal(0, marchTotals.TotalRevenue);
        Assert.Equal(30, marchTotals.TotalExpense);
        Assert.Equal(0, marchTotals.TotalLiability);
    }

    [Fact]
    public async Task GetMonthlyTotalsForDateRange_ThrowsArgumentException_WhenStartDateIsAfterEndDate()
    {
        // Arrange
        var startDate = new DateTime(2024, 4, 1);
        var endDate = new DateTime(2024, 3, 31);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _generalLedgerService.GetMonthlyTotalsForDateRangeAsync(startDate, endDate));
    }
}

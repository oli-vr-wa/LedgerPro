using NSubstitute;
using LedgerPro.Core.Interfaces;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using LedgerPro.Api.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using LedgerPro.Application.DTOs.Common;


namespace LedgerPro.Tests.Api.Extensions;

public class GeneralLedgerEndpointExtensionsTests
{
    private readonly IGeneralLedgerRepository _generalLedgerRepository = Substitute.For<IGeneralLedgerRepository>();

    /// <summary>
    /// Tests that the GetGeneralLedgerItemsAsync method returns an Ok result containing a list of GeneralLedgerItem entities when the repository returns items successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetGeneralLedgerItemsAsync_ReturnsOkResultWithItems()
    {
        // Arrange
        var mockItems = new List<GeneralLedgerItem>
        {
            new GeneralLedgerItem { Id = Guid.NewGuid(), Description = "Tools/Materials purchase - Bunnings", Amount = 562.50m, TransactionDate = DateTime.Now },
            new GeneralLedgerItem { Id = Guid.NewGuid(), Description = "Wages Payment - 24Aug", Amount = 45812.00m, TransactionDate = DateTime.Now }
        };

        _generalLedgerRepository.GetGeneralLedgerItemsAsync().Returns(mockItems);

        // Act
        var result = await GeneralLedgerEndpointExtensions.GetGeneralLedgerItemsAsync(_generalLedgerRepository);

        // Assert
        var okResult = Assert.IsType<Ok<List<GeneralLedgerItem>>>(result);  // Assert that the result is an Ok result containing a list of GeneralLedgerItem
        Assert.NotNull(okResult.Value);                                     // Assert that the returned value is not null
        Assert.Equal(mockItems.Count, okResult.Value.Count);                // Assert that the count of items in the returned list matches the count of mock items

        // Verify that the repository method was called once
        await _generalLedgerRepository.Received(1).GetGeneralLedgerItemsAsync();
    }

    /// <summary>
    /// Tests that the GetGeneralLedgerAccountsAsync method returns an Ok result containing a list of GeneralLedgerAccount entities when the repository 
    /// returns accounts successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetGeneralLedgerAccountsAsync_ReturnsOkResultWithAccounts()
    {
        // Arrange
        var mockAccounts = new List<GeneralLedgerAccount>
        {
            new GeneralLedgerAccount { Id = 5000, Name = "Tools/Materials expense", AccountType = GeneralLedgerAccountType.Expense },
            new GeneralLedgerAccount { Id = 5001, Name = "Equipment expense", AccountType = GeneralLedgerAccountType.Expense }
        };

        _generalLedgerRepository.GetGeneralLedgerAccountsAsync().Returns(mockAccounts);

        // Act
        var result = await GeneralLedgerEndpointExtensions.GetGeneralLedgerAccountsAsync(_generalLedgerRepository);

        // Assert
        var okResult = Assert.IsType<Ok<List<GeneralLedgerAccount>>>(result);  // Assert that the result is an Ok result containing a list of GeneralLedgerAccount
        Assert.NotNull(okResult.Value);                                        // Assert that the returned value is not null
        Assert.Equal(mockAccounts.Count, okResult.Value.Count);                // Assert that the count of accounts in the returned list matches the count of mock accounts

        // Verify that the repository method was called once
        await _generalLedgerRepository.Received(1).GetGeneralLedgerAccountsAsync();
    }

    /// <summary>
    /// Tests that the AddGeneralLedgerAccountAsync method returns an Ok result with a success message when a new general ledger account is added successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddGeneralLedgerAccountAsync_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var newAccount = new GeneralLedgerAccount { Id = 5003, Name = "Office Supplies expense", AccountType = GeneralLedgerAccountType.Expense };

        // Act
        var result = await GeneralLedgerEndpointExtensions.AddGeneralLedgerAccountAsync(newAccount, _generalLedgerRepository);

        // Assert
        var okResult = Assert.IsType<Ok<ActionResponse>>(result);  // Assert that the result is an Ok result containing an object
        Assert.NotNull(okResult.Value);                    // Assert that the returned value is not null

        // Assert that the returned value contains a message indicating success
        var messageProperty = okResult.Value.GetType().GetProperty("Message");
        Assert.NotNull(messageProperty);

        // Verify that the repository method was called once with the correct account
        await _generalLedgerRepository.Received(1).AddGeneralLedgerAccountAsync(newAccount);
    }
}

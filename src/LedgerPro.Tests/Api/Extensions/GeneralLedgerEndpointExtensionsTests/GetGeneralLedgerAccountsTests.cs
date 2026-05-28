using LedgerPro.Api.Extensions;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;

namespace LedgerPro.Tests.Api.Extensions.GeneralLedgerEndpointExtensionsTests;

public class GetGeneralLedgerAccountsTests(WebApplicationFactory<Program> factory) : GeneralLedgerEndpointExtensionsTestsBase(factory)
{
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
}

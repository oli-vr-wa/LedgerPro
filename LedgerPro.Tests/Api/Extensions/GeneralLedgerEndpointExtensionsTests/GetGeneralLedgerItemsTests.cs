using LedgerPro.Api.Extensions;
using LedgerPro.Core.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;

namespace LedgerPro.Tests.Api.Extensions.GeneralLedgerEndpointExtensionsTests;

public class GetGeneralLedgerItemsTests(WebApplicationFactory<Program> factory) : GeneralLedgerEndpointExtensionsTestsBase(factory)
{
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
}

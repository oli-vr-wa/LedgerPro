using LedgerPro.Api.Extensions;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

namespace LedgerPro.Tests.Api.Extensions.BankSourceEndpointExtensionsTests;

public class GetBankSourcesTests : BankSourceEndpointExtensionsTestsBase
{
    /// <summary>
    /// Tests the GetBankSourcesAsync method of the BankSourceEndpointExtensions class to ensure it returns an Ok response containing 
    /// the list of bank sources retrieved from the repository.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetBankSourcesAsync_ReturnsOkWithBankSources()
    {
        // Arrange
        var bankSources = new List<BankSource>
        {
            new BankSource { Id = Guid.NewGuid(), BankName = "Bank A", AccountName = "Account A", AccountNumber = "1234567890", BankType = BankType.NAB },
            new BankSource { Id = Guid.NewGuid(), BankName = "Bank B", AccountName = "Account B", AccountNumber = "0987654321", BankType = BankType.ANZ }
        };

        _bankSourceRepository.GetBankSourcesAsync().Returns(bankSources);

        // Act
        var result = await BankSourceEndpointExtensions.GetBankSourcesAsync(_bankSourceRepository);

        // Assert
        Assert.IsType<Ok<List<BankSource>>>(result);
        var okResult = result as Ok<List<BankSource>>;
        Assert.NotNull(okResult);
        Assert.Equal(bankSources, okResult!.Value);

        // Verify that the repository method was called once
        await _bankSourceRepository.Received(1).GetBankSourcesAsync();
    }
}

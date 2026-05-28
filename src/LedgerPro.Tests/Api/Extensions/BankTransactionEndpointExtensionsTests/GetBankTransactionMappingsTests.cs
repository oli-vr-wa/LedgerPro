using NSubstitute;
using Microsoft.AspNetCore.Mvc.Testing;
using LedgerPro.Core.Entities;
using LedgerPro.Api.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LedgerPro.Tests.Api.Extensions.BankTransactionEndpointExtensionsTests;

public class GetBankTransactionMappingsTests(WebApplicationFactory<Program> factory) : BankTransactionEndpointExtensionsTestsBase(factory)
{
    /// <summary>
    /// Tests the GetBankTransactionMappingsAsync method of the BankTransactionEndpointExtensions class to ensure it returns an Ok result with the expected 
    /// list of bank transaction mappings.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetBankTransactionMappingsAsync_ReturnsOkResultWithMappings()
    {
        // Arrange
        var expectedMappings = new List<BankTransactionMapping>
        {
            new BankTransactionMapping { Id = Guid.NewGuid(), SearchTerm = "Test Mapping 1", TargetGeneralLedgerAccountId = 1000 },
            new BankTransactionMapping { Id = Guid.NewGuid(), SearchTerm = "Test Mapping 2", TargetGeneralLedgerAccountId = 1002 }
        };

        _bankTransactionRepository.GetBankTransactionMappingsAsync().Returns(expectedMappings);

        // Act
        var result = await BankTransactionEndpointExtensions.GetBankTransactionMappingsAsync(_bankTransactionRepository);

        // Assert
        var okResult = Assert.IsType<Ok<List<BankTransactionMapping>>>(result);
        var actualMappings = Assert.IsType<List<BankTransactionMapping>>(okResult.Value, exactMatch: false);
        Assert.Equal(expectedMappings.Count, actualMappings.Count);

        // Verify that the repository method was called
        await _bankTransactionRepository.Received(1).GetBankTransactionMappingsAsync();
    }
}

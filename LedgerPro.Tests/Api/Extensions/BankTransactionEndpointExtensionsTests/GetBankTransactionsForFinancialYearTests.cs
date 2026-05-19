using LedgerPro.Api.Extensions;
using LedgerPro.Application.DTOs.Common;
using LedgerPro.Application.DTOs.Reports;
using LedgerPro.Core.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;

namespace LedgerPro.Tests.Api.Extensions.BankTransactionEndpointExtensionsTests;

public class GetBankTransactionsForFinancialYearTests(WebApplicationFactory<Program> factory) : BankTransactionEndpointExtensionsTestsBase(factory)
{
    /// <summary>
    /// Tests the GetBankTransactionsForFinancialYearAsync method of the BankTransactionEndpointExtensions class to ensure it returns an Ok result 
    /// with the expected list of bank transactions for a given bank source and financial year.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetBankTransactionsForFinancialYear_ReturnsOkResult_WithValidInput()
    {
        // Arrange
        var bankSourceId = Guid.NewGuid();
        var financialYearEnding = 2023;
        
        var expectedTransactions = new List<BankTransactionRowDto>
        {
            new BankTransactionRowDto(Guid.NewGuid(), new DateTime(2023, 1, 15), "Test Transaction 1", -200, "Transfer Out", BankTransactionStatus.Pending, string.Empty),
            new BankTransactionRowDto(Guid.NewGuid(), new DateTime(2023, 2, 20), "Test Transaction 2", 2400, "Transfer In", BankTransactionStatus.Categorized, "Income - Sales Revenue")
        };

        _bankTransactionRepository.GetBankTransactionRowsAsync(bankSourceId, financialYearEnding).Returns(expectedTransactions);

        // Act
        var response = await BankTransactionEndpointExtensions.GetBankTransactionsForFinancialYearAsync(bankSourceId, financialYearEnding, _bankTransactionRepository);

        // Assert
        var okResult = Assert.IsType<Ok<List<BankTransactionRowDto>>>(response);
        var returnedTransactions = Assert.IsType<List<BankTransactionRowDto>>(okResult.Value);
        Assert.Equal(expectedTransactions.Count, returnedTransactions.Count);
        Assert.Equal(expectedTransactions[0].Description, returnedTransactions[0].Description);
        Assert.Equal(expectedTransactions[1].Amount, returnedTransactions[1].Amount);        
    }

    /// <summary>
    /// Tests the GetBankTransactionsForFinancialYearAsync method to ensure it returns a BadRequest result when an invalid financial year is provided.
    /// The test verifies that the validation logic correctly identifies financial years that are outside the acceptable range (e.g., before 1900 or after 2100) 
    /// and that the repository method is not called when validation fails.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetBankTransactionsForFinancialYear_ReturnsBadRequest_ForInvalidFinancialYear()
    {
        // Arrange
        var bankSourceId = Guid.NewGuid();
        var invalidFinancialYearEndingLow = 1800; // Invalid year        
        var invalidFinancialYearEndingHigh = 2200; // Invalid year
        string expectedErrorMessage = "Financial year must be between 1900 and 2100.";

        // Act
        var responseLow = await BankTransactionEndpointExtensions.GetBankTransactionsForFinancialYearAsync(bankSourceId, invalidFinancialYearEndingLow, _bankTransactionRepository);
        var responseHigh = await BankTransactionEndpointExtensions.GetBankTransactionsForFinancialYearAsync(bankSourceId, invalidFinancialYearEndingHigh, _bankTransactionRepository);

        // Assert
        var badRequestLow = Assert.IsType<BadRequest<ErrorResponse>>(responseLow);
        Assert.Equal(expectedErrorMessage, badRequestLow!.Value!.ValidationErrors!.First().Value.First());

        var badRequestHigh = Assert.IsType<BadRequest<ErrorResponse>>(responseHigh);
        Assert.Equal(expectedErrorMessage, badRequestHigh!.Value!.ValidationErrors!.First().Value.First());
        
        // Verify that the repository method was not called due to validation failure
        await _bankTransactionRepository.DidNotReceive().GetBankTransactionRowsAsync(Arg.Any<Guid>(), Arg.Any<int?>());
    }
}

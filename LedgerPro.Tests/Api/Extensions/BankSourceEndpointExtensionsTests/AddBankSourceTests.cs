using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LedgerPro.Api.Extensions;
using LedgerPro.Application.DTOs.Common;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

namespace LedgerPro.Tests.Api.Extensions.BankSourceEndpointExtensionsTests;

public class AddBankSourceTests : BankSourceEndpointExtensionsTestsBase
{
    /// <summary>
    /// Tests the AddBankSourceAsync method of the BankSourceEndpointExtensions class to ensure it returns 
    /// a Bad Request response with the expected error message when the bank source parameter is null.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddBankSourceAsync_ReturnsBadRequest_WhenBankSourceIsNull()
    {
        // Act
        var result = await BankSourceEndpointExtensions.AddBankSourceAsync(null!, _bankSourceRepository);

        // Assert
        Assert.IsType<BadRequest<ErrorResponse>>(result);
        var badRequest = result as BadRequest<ErrorResponse>;
        Assert.NotNull(badRequest);
        Assert.Equal("Bank source data is required.", badRequest!.Value!.Error);

        // Verify that the repository method was not called
        await _bankSourceRepository.DidNotReceive().IsBankSourceNameInUseAsync(Arg.Any<string>());
        await _bankSourceRepository.DidNotReceive().AddBankSourceAsync(Arg.Any<BankSource>());
    }

    /// <summary>
    /// Tests the AddBankSourceAsync method of the BankSourceEndpointExtensions class to ensure it returns 
    /// a Bad Request response with the expected error message when the bank source name is already in use.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddBankSourceAsync_ReturnsBadRequest_WhenBankSourceNameIsInUse()
    {
        // Arrange
        var bankSource = new BankSource { Id = Guid.NewGuid(), BankName = "Bank A", AccountName = "Account A", AccountNumber = "1234567890", BankType = BankType.NAB };
        _bankSourceRepository.IsBankSourceNameInUseAsync(bankSource.BankName).Returns(true);

        // Act
        var result = await BankSourceEndpointExtensions.AddBankSourceAsync(bankSource, _bankSourceRepository);

        // Assert
        Assert.IsType<BadRequest<ErrorResponse>>(result);
        var badRequest = result as BadRequest<ErrorResponse>;
        Assert.NotNull(badRequest);
        Assert.Equal($"Bank source with name '{bankSource.BankName}' is already in use and cannot be added.", badRequest!.Value!.Error);

        // Verify that the repository method was called once
        await _bankSourceRepository.Received(1).IsBankSourceNameInUseAsync(bankSource.BankName);
        await _bankSourceRepository.DidNotReceive().AddBankSourceAsync(Arg.Any<BankSource>());
    }

    /// <summary>
    /// Tests the AddBankSourceAsync method of the BankSourceEndpointExtensions class to ensure it returns 
    /// a Created response with the expected location and value when the bank source is valid.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddBankSourceAsync_ReturnsCreated_WhenBankSourceIsValid()
    {
        // Arrange
        var bankSource = new BankSource { Id = Guid.NewGuid(), BankName = "Bank A", AccountName = "Account A", AccountNumber = "1234567890", BankType = BankType.NAB };
        _bankSourceRepository.IsBankSourceNameInUseAsync(bankSource.BankName).Returns(false);

        // Act
        var result = await BankSourceEndpointExtensions.AddBankSourceAsync(bankSource, _bankSourceRepository);

        // Assert
        Assert.IsType<Created<BankSource>>(result);
        var createdResult = result as Created<BankSource>;
        Assert.NotNull(createdResult);
        Assert.Equal($"/api/v1/banksources/{bankSource.Id}", createdResult!.Location);
        Assert.Equal(bankSource, createdResult.Value);

        // Verify that the repository methods were called once
        await _bankSourceRepository.Received(1).IsBankSourceNameInUseAsync(bankSource.BankName);
        await _bankSourceRepository.Received(1).AddBankSourceAsync(bankSource);
    }
}

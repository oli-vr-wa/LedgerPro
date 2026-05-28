using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LedgerPro.Api.Extensions;
using LedgerPro.Application.DTOs.BankSource;
using LedgerPro.Application.DTOs.Common;
using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;

namespace LedgerPro.Tests.Api.Extensions.BankSourceEndpointExtensionsTests;

public class AddBankSourceTests : BankSourceEndpointExtensionsTestsBase
{    
    /// <summary>
    /// Tests the AddBankSourceAsync method of the BankSourceEndpointExtensions class to ensure it returns 
    /// a Created response with the expected location and value when the bank source is valid.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddBankSourceAsync_ReturnsCreated_WhenRequestAndBankSourceAreValid()
    {
        // Arrange
        var request = new AddBankSourceRequest(
            AccountName: "Test Account",
            AccountNumber: "1234567890",
            BankName: "Test Bank",
            BankType: BankType.NAB
        );

        _bankSourceService.AddBankSourceAsync(request).Returns(Guid.NewGuid());
        _unitOfWork.CommitAsync().Returns(1);

        // Act
        var result = await BankSourceEndpointExtensions.AddBankSourceAsync(request, _bankSourceService, _unitOfWork);

        // Assert
        Assert.IsType<Created<Guid>>(result);
        var createdResult = result as Created<Guid>;
        Assert.NotNull(createdResult);
        Assert.Equal($"/api/v1/banksources/{createdResult!.Value}", createdResult.Location);

        // Verify that the repository methods were called once
        await _bankSourceService.Received(1).AddBankSourceAsync(request);
        await _unitOfWork.Received(1).CommitAsync();
    }

    [Fact]
    public async Task AddBankSourceAsync_ReturnsBadRequest_WhenRequestIsNull()
    {
        // Act
        var result = await BankSourceEndpointExtensions.AddBankSourceAsync(null!, _bankSourceService, _unitOfWork);

        // Assert
        Assert.IsType<BadRequest<ErrorResponse>>(result);
        var badRequestResult = result as BadRequest<ErrorResponse>;
        Assert.NotNull(badRequestResult);
        Assert.Equal("Bank source data is required.", badRequestResult!.Value!.Error);
        // Verify that the service method was not called
        await _bankSourceService.DidNotReceive().AddBankSourceAsync(Arg.Any<AddBankSourceRequest>());
        // Verify that the unit of work method was not called
        await _unitOfWork.DidNotReceive().CommitAsync();
    }
}

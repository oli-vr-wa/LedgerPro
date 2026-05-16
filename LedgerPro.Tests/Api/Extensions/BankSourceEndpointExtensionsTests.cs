using LedgerPro.Application.Interfaces;
using LedgerPro.Core.Interfaces;
using LedgerPro.Application.DTOs.BankStatement;
using NSubstitute;
using LedgerPro.Api.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using LedgerPro.Application.DTOs.Common;
using LedgerPro.Core.Common;

namespace LedgerPro.Tests.Api.Extensions;

public class BankSourceEndpointExtensionsTests
{
    private readonly IBankImportService _bankImportService = Substitute.For<IBankImportService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    /// <summary>
    /// Tests the ImportBankStatementAsync method of the BankSourceEndpointExtensions class to ensure it returns a Bad Request response with 
    /// the expected error message when the file is null.
    /// The test arranges the necessary parameters, invokes the method with a null file, and asserts that the response is a Bad Request containing 
    /// the correct error message. It also verifies that the bank import service's ImportBankStatementAsync method was not called and that the unit of work's CommitAsync 
    /// method was not called, ensuring that no processing occurs when the file is null.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ImportBankStatementAsync_ShouldReturnBadRequest_WhenFileIsNull()
    {
        // Arrange
        Guid bankSourceId = Guid.NewGuid();

        // Act
        var result = await BankSourceEndpointExtensions.ImportBankStatementAsync(bankSourceId, null!, _bankImportService, _unitOfWork);

        // Assert
        Assert.IsType<BadRequest<ErrorResponse>>(result);
        var badRequest = result as BadRequest<ErrorResponse>;
        Assert.NotNull(badRequest);
        Assert.Equal("No file uploaded.", badRequest!.Value!.Error); 

        // Verify that the service method was not called and that the unit of work's CommitAsync method was not called
        await _bankImportService.DidNotReceive().ImportBankStatementAsync(Arg.Any<UploadBankStatementRequest>());
        await _unitOfWork.DidNotReceive().CommitAsync();
    }

    /// <summary>
    /// Tests the ImportBankStatementAsync method of the BankSourceEndpointExtensions class to ensure it returns a Bad Request response with the expected error message
    /// when the bank import service returns a failure indicating that the bank source was not found.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ImportBankStatementAsync_ReturnsBadRequest_WhenServiceReturnsBankSourceNotFoundFailure()
    {
        // Arrange
        Guid bankSourceId = Guid.NewGuid();
        var fileMock = Substitute.For<IFormFile>();
        fileMock.Length.Returns(1); // Simulate a non-empty file        

        _bankImportService.ImportBankStatementAsync(Arg.Any<UploadBankStatementRequest>())
            .Returns(Result<int>.Failure($"Bank source with ID {bankSourceId} not found."));
        
        // Act
        var result = await BankSourceEndpointExtensions.ImportBankStatementAsync(bankSourceId, fileMock, _bankImportService, _unitOfWork);

        // Assert
        Assert.IsType<BadRequest<ErrorResponse>>(result);
        var badRequest = result as BadRequest<ErrorResponse>;
        Assert.NotNull(badRequest);
        Assert.Equal($"Bank source with ID {bankSourceId} not found.", badRequest!.Value!.Error);

        // Verify that the service method was called once and that the unit of work's CommitAsync method was not called
        await _bankImportService.Received(1).ImportBankStatementAsync(Arg.Any<UploadBankStatementRequest>());
        await _unitOfWork.DidNotReceive().CommitAsync();
    }

    /// <summary>
    /// Tests the ImportBankStatementAsync method of the BankSourceEndpointExtensions class to ensure it returns an Ok response with 
    /// the expected message when the bank import service returns a success result.
    /// The test arranges the necessary parameters, including a mock file and a successful result from the bank import service, invokes the method, 
    /// and asserts that the response is an Ok result containing the correct success message. It also verifies that the bank import service's 
    /// ImportBankStatementAsync method was called once and that the
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ImportBankStatementAsync_ReturnsOk_WhenServiceReturnsSuccess()
    {
        // Arrange
        Guid bankSourceId = Guid.NewGuid();
        var fileMock = Substitute.For<IFormFile>();
        fileMock.Length.Returns(1); // Simulate a non-empty file

        _bankImportService.ImportBankStatementAsync(Arg.Any<UploadBankStatementRequest>())
            .Returns(Result<int>.Success(1));

        // Act
        var result = await BankSourceEndpointExtensions.ImportBankStatementAsync(bankSourceId, fileMock, _bankImportService, _unitOfWork);

        // Assert
        Assert.IsType<Ok<ImportBankStatementResponse>>(result);
        var okResult = result as Ok<ImportBankStatementResponse>;
        Assert.NotNull(okResult);
        Assert.Equal("Bank statement imported successfully.", okResult!.Value!.Message);

        // Verify that the service method was called once and that the unit of work's CommitAsync method was called once
        await _bankImportService.Received(1).ImportBankStatementAsync(Arg.Any<UploadBankStatementRequest>());
        await _unitOfWork.Received(1).CommitAsync();
    }
}

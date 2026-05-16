using LedgerPro.Application.Interfaces;
using LedgerPro.Core.Interfaces;
using LedgerPro.Application.DTOs.BankStatement;
using NSubstitute;
using LedgerPro.Api.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using LedgerPro.Application.DTOs.Common;
using LedgerPro.Core.Common;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;

namespace LedgerPro.Tests.Api.Extensions;

public class BankSourceEndpointExtensionsTests
{
    private readonly IBankImportService _bankImportService = Substitute.For<IBankImportService>();
    private readonly IBankSourceRepository _bankSourceRepository = Substitute.For<IBankSourceRepository>();
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

using System.Net;
using System.Net.Http.Json;
using LedgerPro.Api.Extensions;
using LedgerPro.Application.DTOs.Common;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Exceptions;
using LedgerPro.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace LedgerPro.Tests.Api.Extensions;

public class BankTransactionEndPointExtensionsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IBankTransactionRepository _bankTransactionRepository = Substitute.For<IBankTransactionRepository>();
    private readonly IBankTransactionService _bankTransactionService = Substitute.For<IBankTransactionService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    public BankTransactionEndPointExtensionsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Replace the actual services with the mocked ones
                services.AddScoped(_ => _bankTransactionRepository);
                services.AddScoped(_ => _bankTransactionService);
                services.AddScoped(_ => _unitOfWork);
            });
        });
    }

    /// <summary>
    /// Tests the GetBankTransactionsAsync method of the BankTransactionEndpointExtensions class to ensure it returns an Ok result with the expected 
    /// list of bank transactions for a given bank source ID. The test sets up the repository to return a predefined list of transactions, invokes the method, 
    /// and asserts that the result is an Ok result containing the correct transactions. It also verifies that the repository method was called with the correct bank source ID.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetBankTransactionsAsync_ReturnsOkResultWithTransactions()
    {
        // Arrange
        var bankSourceId = Guid.NewGuid();
        var expectedTransactions = new List<BankTransaction>
        {
            new BankTransaction { Id = Guid.NewGuid(), BankSourceId = bankSourceId, Amount = 100, Description = "Test Transaction 1" },
            new BankTransaction { Id = Guid.NewGuid(), BankSourceId = bankSourceId, Amount = 200, Description = "Test Transaction 2" }
        };

        _bankTransactionRepository.GetBankTransactionsAsync(bankSourceId).Returns(expectedTransactions);

        // Act
        var result = await BankTransactionEndpointExtensions.GetBankTransactionsAsync(bankSourceId, _bankTransactionRepository);

        // Assert
        var okResult = Assert.IsType<Ok<List<BankTransaction>>>(result);
        var actualTransactions = Assert.IsType<List<BankTransaction>>(okResult.Value, exactMatch: false);
        Assert.Equal(expectedTransactions.Count, actualTransactions.Count);

        // Verify that the repository method was called with the correct bank source ID
        await _bankTransactionRepository.Received(1).GetBankTransactionsAsync(bankSourceId);
    }

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

    /// <summary>
    /// Tests the AddBankTransactionMappingAsync method of the BankTransactionEndpointExtensions class to ensure it returns a 
    /// Created result with the success message when a new bank transaction mapping is added. The test sets up the service to accept the new mapping, invokes the method,
    /// and verifies the result and interactions with the service and unit of work.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddBankTransactionMappingAsync_ReturnsCreatedResult()
    {
        // Arrange
        var mapping = new BankTransactionMapping { Id = Guid.NewGuid(), SearchTerm = "Test Mapping", TargetGeneralLedgerAccountId = 1000 };

        // Act
        var result = await BankTransactionEndpointExtensions.AddBankTransactionMappingAsync(mapping, _bankTransactionService, _unitOfWork);

        // Assert
        var createdResult = Assert.IsType<Created<ActionResponse>>(result);
        Assert.Equal($"/api/v1/banktransactions/mappings/{mapping.Id}", createdResult.Location);
        var actionResponse = Assert.IsType<ActionResponse>(createdResult.Value);
        Assert.Equal("Bank transaction mapping added successfully.", actionResponse.Message);

        // Verify that the service method was called with the correct mapping
        await _bankTransactionService.Received(1).AddBankTransactionMappingAsync(mapping);

        // Verify that the unit of work's CommitAsync method was called
        await _unitOfWork.Received(1).CommitAsync();
    }

    /// <summary>
    /// Tests the AddBankTransactionMappingAsync method of the BankTransactionEndpointExtensions class to ensure it throws an 
    /// ArgumentNullException with the expected error message when the mapping parameter is null. 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddBankTransactionMappingAsync_ThrowsBadRequest_WhenMappingIsNull()
    {
        // Arrange
        BankTransactionMapping mapping = null!;

        string expectedErrorMessage = "The bank transaction mapping cannot be null";
        _bankTransactionService.AddBankTransactionMappingAsync(mapping)
            .ThrowsAsync(new ArgumentNullException(nameof(mapping), expectedErrorMessage));
        
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => 
            BankTransactionEndpointExtensions.AddBankTransactionMappingAsync(mapping, _bankTransactionService, _unitOfWork));

        Assert.Equal($"{expectedErrorMessage} (Parameter 'mapping')", exception.Message);
        // Verify that the service method was called with the null mapping
        await _bankTransactionService.Received(1).AddBankTransactionMappingAsync(mapping);
        await _unitOfWork.DidNotReceive().CommitAsync();        
    }

    /// <summary>
    /// Tests the AddBankTransactionMappingAsync method of the BankTransactionEndpointExtensions class to ensure it throws a BusinessException 
    /// with the expected error message when the mapping is a duplicate.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddBankTransactionMappingAsync_ThrowsBadRequest_WhenMappingIsDuplicate()
    {
        // Arrange
        var mapping = new BankTransactionMapping { Id = Guid.NewGuid(), SearchTerm = "Test Mapping - duplicated", TargetGeneralLedgerAccountId = 1000 };
        string expectedErrorMessage = "The bank transaction mapping already exists.";

        _bankTransactionService.AddBankTransactionMappingAsync(mapping)
            .ThrowsAsync(new BusinessException(expectedErrorMessage));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => 
            BankTransactionEndpointExtensions.AddBankTransactionMappingAsync(mapping, _bankTransactionService, _unitOfWork));

        Assert.Equal(expectedErrorMessage, exception.Message);
        // Verify that the service method was called with the correct mapping
        await _bankTransactionService.Received(1).AddBankTransactionMappingAsync(mapping);
        await _unitOfWork.DidNotReceive().CommitAsync();        
    }

    /// <summary>
    /// Tests the AddBankTransactionMappingAsync method of the BankTransactionEndpointExtensions class to ensure it returns a Bad Request response with 
    /// the expected error message when the service throws an ArgumentNullException. 
    /// The test sets up the service to throw the exception, invokes the method, and asserts that the response has a 400 status code and contains 
    /// the correct error message in the problem details. It also verifies that the service method was called and 
    /// that the unit of work's CommitAsync method was not called.    
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddBankTransactionMappingAsync_Returns400BadRequest_WhenServiceThrowsArgumentNullException()
    {
        // Arrange
        var client = _factory.CreateClient();

        var mapping = new BankTransactionMapping { Id = Guid.NewGuid(), SearchTerm = "Test Mapping", TargetGeneralLedgerAccountId = 1000 };
        string expectedErrorMessage = "The bank transaction mapping cannot be null";

        _bankTransactionService.AddBankTransactionMappingAsync(Arg.Any<BankTransactionMapping>())
            .ThrowsAsync(new ArgumentNullException(nameof(mapping), expectedErrorMessage));

        // Act 
        var response = await client.PostAsJsonAsync("/api/v1/banktransactions/mappings", mapping);

        // Assert        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.Equal("Something went wrong - Bad Request", problemDetails.Title);
        Assert.Equal($"{expectedErrorMessage} (Parameter 'mapping')", problemDetails.Detail);

        await _bankTransactionService.Received(1).AddBankTransactionMappingAsync(Arg.Any<BankTransactionMapping>());
        await _unitOfWork.DidNotReceive().CommitAsync();
    }

    /// <summary>
    /// Tests the AddBankTransactionMappingAsync method of the BankTransactionEndpointExtensions class to ensure it returns a Bad Request response with 
    /// the expected error message when the service throws a BusinessException.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddBankTransactionMappingAsync_Returns400BadRequest_WhenServiceThrowsBusinessException()
    {
        // Arrange
        var client = _factory.CreateClient();

        var mapping = new BankTransactionMapping { Id = Guid.NewGuid(), SearchTerm = "Test Mapping - duplicated", TargetGeneralLedgerAccountId = 1000 };
        string expectedErrorMessage = "The bank transaction mapping already exists.";

        _bankTransactionService.AddBankTransactionMappingAsync(Arg.Any<BankTransactionMapping>())
            .ThrowsAsync(new BusinessException(expectedErrorMessage));

        // Act 
        var response = await client.PostAsJsonAsync("/api/v1/banktransactions/mappings", mapping);

        // Assert        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.Equal("Business Rules Violation - Bad Request", problemDetails.Title);
        Assert.Equal(expectedErrorMessage, problemDetails.Detail);

        await _bankTransactionService.Received(1).AddBankTransactionMappingAsync(Arg.Any<BankTransactionMapping>());
        await _unitOfWork.DidNotReceive().CommitAsync();
    }

    /// <summary>
    /// Tests the AddBankTransactionMappingAsync method of the BankTransactionEndpointExtensions class to ensure it successfully adds 
    /// a new bank transaction mapping and returns a Created result.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddBankTransactionMappingAsync_SuccessfullyAddsMapping()
    {
        // Arrange
        var client = _factory.CreateClient();

        var mapping = new BankTransactionMapping { Id = Guid.NewGuid(), SearchTerm = "Test Mapping", TargetGeneralLedgerAccountId = 1000 };
        _bankTransactionService.AddBankTransactionMappingAsync(Arg.Any<BankTransactionMapping>()).Returns(Task.CompletedTask);
        _unitOfWork.CommitAsync().Returns(1);

        // Act 
        var response = await client.PostAsJsonAsync("/api/v1/banktransactions/mappings", mapping);

        // Assert        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var actionResponse = await response.Content.ReadFromJsonAsync<ActionResponse>();
        Assert.NotNull(actionResponse);
        Assert.Equal("Bank transaction mapping added successfully.", actionResponse.Message);
        Assert.Equal($"/api/v1/banktransactions/mappings/{mapping.Id}", response.Headers.Location?.ToString());

        await _bankTransactionService.Received(1).AddBankTransactionMappingAsync(Arg.Any<BankTransactionMapping>());
        await _unitOfWork.Received(1).CommitAsync();
    }
}

using System.Net.Http.Json;
using LedgerPro.Api.Extensions;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using LedgerPro.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace LedgerPro.Tests.Api.Extensions.GeneralLedgerEndpointExtensionsTests;

public class AddGeneralLedgerAccountTests(WebApplicationFactory<Program> factory) : GeneralLedgerEndpointExtensionsTestsBase(factory)
{
    /// <summary>
    /// Tests that the AddGeneralLedgerAccountAsync method returns an Ok result with a success message when a new general ledger account is added successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddGeneralLedgerAccountAsync_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var newAccount = new GeneralLedgerAccount { Id = 5003, Name = "Office Supplies expense", AccountType = GeneralLedgerAccountType.Expense };
        _generalLedgerService.AddGeneralLedgerAccountAsync(newAccount).Returns(Task.CompletedTask);  // Set up the service to complete successfully when adding the account
        _unitOfWork.CommitAsync().Returns(1);  // Set up the unit of work to complete successfully when committing the transaction 

        // Act
        var result = await GeneralLedgerEndpointExtensions.AddGeneralLedgerAccountAsync(newAccount, _generalLedgerService, _unitOfWork);

        // Assert
        var createdResult = Assert.IsType<Created<GeneralLedgerAccount>>(result);   // Assert that the result is a Created result containing a GeneralLedgerAccount
        Assert.NotNull(createdResult.Value);                                        // Assert that the returned value is not null
        Assert.Equal(newAccount.Id, createdResult.Value.Id);                        // Assert that the returned account has the same ID as the new account
        Assert.Equal(newAccount.Name, createdResult.Value.Name);                    // Assert that the returned account has the same Name as the new account
        Assert.Equal(newAccount.AccountType, createdResult.Value.AccountType);      // Assert that the returned account has the same AccountType as the new account

        // Verify that the service method was called once with the correct account        
        await _generalLedgerService.Received(1).AddGeneralLedgerAccountAsync(newAccount);
        // Verify that the unit of work's CommitAsync method was called once
        await _unitOfWork.Received(1).CommitAsync();
    }

    /// <summary>
    /// Tests that the AddGeneralLedgerAccountAsync method returns a BadRequest result with an error message when attempting to add a 
    /// general ledger account with an ID that is already in use.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddGeneralLedgerAccountAsync_ReturnsBadRequest_WhenAccountIdInUse()
    {
        // Arrange
        var newAccount = new GeneralLedgerAccount { Id = 5000, Name = "Duplicate Account", AccountType = GeneralLedgerAccountType.Expense };

        string expectedErrorMessage = $"General ledger account with ID {newAccount.Id} is already in use and cannot be added.";

        // Set up the service to return a failure result indicating the account ID is already in use
        _generalLedgerService.AddGeneralLedgerAccountAsync(newAccount).ThrowsAsync(new BusinessException(expectedErrorMessage));

        // Act - We expect an exception to be thrown since the service will throw a BusinessException when attempting to add an account with an ID that is already in use
        var exception = await Assert.ThrowsAsync<BusinessException>(() => GeneralLedgerEndpointExtensions.AddGeneralLedgerAccountAsync(newAccount, _generalLedgerService, _unitOfWork));

        // Assert
        Assert.Equal(expectedErrorMessage, exception.Message);    // Assert that the exception message matches the expected error message        

        // Verify that the service method was called once with the correct account
        await _generalLedgerService.Received(1).AddGeneralLedgerAccountAsync(newAccount);  
        await _unitOfWork.DidNotReceive().CommitAsync();  // Verify that the unit of work's CommitAsync method was not called since the operation should have failed before reaching the commit step      
    }

    /// <summary>
    /// Tests that the AddGeneralLedgerAccountAsync method returns a BadRequest result with an error message when the service 
    /// throws an exception.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddGeneralLedgerAccountAsync_ReturnsBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var newAccount = new GeneralLedgerAccount { Id = 5004, Name = "New Account", AccountType = GeneralLedgerAccountType.Expense };

        // Set up the service to throw an exception when attempting to add the account
        _generalLedgerService.AddGeneralLedgerAccountAsync(newAccount)
            .ThrowsAsync(new ArgumentException("Failed to create account", nameof(newAccount)));

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => GeneralLedgerEndpointExtensions.AddGeneralLedgerAccountAsync(newAccount, _generalLedgerService, _unitOfWork));

        // Assert
        Assert.Equal("Failed to create account (Parameter 'newAccount')", exception.Message);    // Assert that the exception message matches the expected error message
        Assert.Equal(nameof(newAccount), exception.ParamName);         // Assert that the parameter name in the exception matches the name of the new account parameter

        // Verify that the service method was called once with the correct account
        await _generalLedgerService.Received(1).AddGeneralLedgerAccountAsync(newAccount);
        await _unitOfWork.DidNotReceive().CommitAsync();  // Verify that the unit of work's CommitAsync method was not called since the operation should have failed before reaching the commit step
    }

    /// <summary>
    /// Tests that the AddGeneralLedgerAccountAsync method returns a BadRequest result with an error message when the service throws a BusinessException,
    /// which is expected to be handled by the global exception handler and result in a standardized ProblemDetails response with a 400 Bad Request status code. 
    /// This test verifies that the global exception handling mechanism is correctly translating exceptions thrown by the service into appropriate HTTP responses. 
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddGeneralLedgerAccountAsync_Returns400BadRequest_WhenServiceThrowsBusinessException()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newAccount = new GeneralLedgerAccount { Id = 5004, Name = "Test Account", AccountType = GeneralLedgerAccountType.Expense }; 

        // Force the service to throw a BusinessException when attempting to add the account
        string expectedErrorMessage = $"General ledger account with ID {newAccount.Id} is already in use and cannot be added.";

        // When mocking the service need to use Arg.Any<GeneralLedgerAccount>() to match any account parameter 
        // since the actual account object passed to the service will be a different instance than the newAccount object defined in the test, 
        // even though they have the same property values.This happens as the object will be deserialized from the HTTP request body and will 
        // not be the same reference as the newAccount object created in the test method.
        _generalLedgerService.AddGeneralLedgerAccountAsync(Arg.Any<GeneralLedgerAccount>())
            .ThrowsAsync(new BusinessException(expectedErrorMessage));
        
        // Act
        var response = await client.PostAsJsonAsync("/api/v1/ledger/account", newAccount);

        // Assert
        // Verify that the response status code is 400 Bad Request
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        // Verify the response content matches the standardized ProblemDetails format 
        //var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);  // Assert that the problem details object is not null
        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);  // Assert that the status code in the problem details is 400
        Assert.Equal("Business Rules Violation - Bad Request", problemDetails.Title);  // Assert that the title in the problem details matches the expected title for a business exception
        Assert.Equal(expectedErrorMessage, problemDetails.Detail);  // Assert that the detail in the problem details matches the expected error message

        await _unitOfWork.DidNotReceive().CommitAsync();  // Verify that the unit of work's CommitAsync method was not called since the operation should have failed before reaching the commit step
    }
}

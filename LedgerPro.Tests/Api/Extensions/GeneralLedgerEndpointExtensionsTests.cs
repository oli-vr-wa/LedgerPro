using NSubstitute;
using LedgerPro.Core.Common;
using LedgerPro.Core.Interfaces;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using LedgerPro.Api.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using LedgerPro.Application.DTOs.Common;
using NSubstitute.ExceptionExtensions;


namespace LedgerPro.Tests.Api.Extensions;

public class GeneralLedgerEndpointExtensionsTests
{
    private readonly IGeneralLedgerRepository _generalLedgerRepository = Substitute.For<IGeneralLedgerRepository>();
    private readonly IGeneralLedgerService _generalLedgerService = Substitute.For<IGeneralLedgerService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

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

    /// <summary>
    /// Tests that the GetGeneralLedgerAccountsAsync method returns an Ok result containing a list of GeneralLedgerAccount entities when the repository 
    /// returns accounts successfully.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetGeneralLedgerAccountsAsync_ReturnsOkResultWithAccounts()
    {
        // Arrange
        var mockAccounts = new List<GeneralLedgerAccount>
        {
            new GeneralLedgerAccount { Id = 5000, Name = "Tools/Materials expense", AccountType = GeneralLedgerAccountType.Expense },
            new GeneralLedgerAccount { Id = 5001, Name = "Equipment expense", AccountType = GeneralLedgerAccountType.Expense }
        };

        _generalLedgerRepository.GetGeneralLedgerAccountsAsync().Returns(mockAccounts);

        // Act
        var result = await GeneralLedgerEndpointExtensions.GetGeneralLedgerAccountsAsync(_generalLedgerRepository);

        // Assert
        var okResult = Assert.IsType<Ok<List<GeneralLedgerAccount>>>(result);  // Assert that the result is an Ok result containing a list of GeneralLedgerAccount
        Assert.NotNull(okResult.Value);                                        // Assert that the returned value is not null
        Assert.Equal(mockAccounts.Count, okResult.Value.Count);                // Assert that the count of accounts in the returned list matches the count of mock accounts

        // Verify that the repository method was called once
        await _generalLedgerRepository.Received(1).GetGeneralLedgerAccountsAsync();
    }

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
    /*[Fact]
    public async Task AddGeneralLedgerAccountAsync_ReturnsBadRequest_WhenAccountIdInUse()
    {
        // Arrange
        var newAccount = new GeneralLedgerAccount { Id = 5000, Name = "Duplicate Account", AccountType = GeneralLedgerAccountType.Expense };

        string expectedErrorMessage = $"General ledger account with ID {newAccount.Id} is already in use and cannot be added.";

        // Set up the service to return a failure result indicating the account ID is already in use
        _generalLedgerService.AddGeneralLedgerAccountAsync(newAccount).ThrowsAsync(new InvalidOperationException(expectedErrorMessage));

        // Act
        var result = await GeneralLedgerEndpointExtensions.AddGeneralLedgerAccountAsync(newAccount, _generalLedgerService, _unitOfWork);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest<ErrorResponse>>(result);    // Assert that the result is a BadRequest result containing an ErrorResponse
        Assert.NotNull(badRequestResult.Value);                                     // Assert that the returned value is not
        Assert.Equal(expectedErrorMessage, badRequestResult.Value.Error);           // Assert that the error message matches the expected message
     
        // Verify that the service method was called once with the correct account
        await _generalLedgerService.Received(1).AddGeneralLedgerAccountAsync(newAccount);        
    }*/

    /// <summary>
    /// Tests that the AddGeneralLedgerAccountAsync method returns a BadRequest result with an error message when the service 
    /// throws an exception.
    /// </summary>
    /// <returns></returns>
    /*[Fact]
    public async Task AddGeneralLedgerAccountAsync_ReturnsBadRequest_WhenServiceThrowsException()
    {
        // Arrange
        var newAccount = new GeneralLedgerAccount { Id = 5004, Name = "New Account", AccountType = GeneralLedgerAccountType.Expense };

        // Set up the service to throw an exception when attempting to add the account
        _generalLedgerService.AddGeneralLedgerAccountAsync(newAccount)
            .ThrowsAsync(new ArgumentException("Failed to create account", nameof(newAccount)));

        // Act
        var result = await GeneralLedgerEndpointExtensions.AddGeneralLedgerAccountAsync(newAccount, _generalLedgerService, _unitOfWork);

        // Assert
        var badRequestResult = Assert.IsType<BadRequest<ErrorResponse>>(result);    // Assert that the result is a BadRequest result containing an ErrorResponse
        Assert.NotNull(badRequestResult.Value);                                     // Assert that the returned value is not
        Assert.Equal("Failed to create account", badRequestResult.Value.Error);   // Assert that the error message matches the expected message        

        // Verify that the service method was called once with the correct account
        await _generalLedgerService.Received(1).AddGeneralLedgerAccountAsync(newAccount);
    }*/
}

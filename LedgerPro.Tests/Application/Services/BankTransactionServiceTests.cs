using NSubstitute;
using LedgerPro.Core.Interfaces;
using LedgerPro.Application.Services;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using NSubstitute.ExceptionExtensions;
using LedgerPro.Core.Exceptions;

namespace LedgerPro.Tests.Application.Services;

public class BankTransactionServiceTests
{
    private readonly IBankTransactionRepository _bankTransactionRepository = Substitute.For<IBankTransactionRepository>();

    private readonly BankTransactionService _bankTransactionService;

    public BankTransactionServiceTests()
    {
        _bankTransactionService = new BankTransactionService(_bankTransactionRepository);
    }

    /// <summary>
    /// Tests that when a duplicate bank transaction mapping is detected, the service throws a BusinessException with the appropriate error message, 
    /// and that the repository's AddBankTransactionMappingAsync method is not called to add the duplicate mapping.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddBankTransactionMappingAsync_ThrowsBusinessException_WhenDuplicateExists()
    {
        // Arrange
        var mapping = new BankTransactionMapping
        {
            Id = Guid.NewGuid(),
            SearchTerm = "Duplicate Mapping",
            DescriptionTemplate = "This is a duplicate mapping.",
            ReferenceTemplate = "TEST",
            TargetGeneralLedgerAccountId = 5000,
            Priority = 1,
            MatchStrategy = BankTransactionMatchStrategy.Exact
        };
        
        _bankTransactionRepository.IsBankTransactionMappingDuplicateAsync(mapping).Returns(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BusinessException>(() => _bankTransactionService.AddBankTransactionMappingAsync(mapping));
        // Assert that the exception message is correct
        Assert.Equal("The bank transaction mapping already exists.", exception.Message);
        // Verify that the AddBankTransactionMappingAsync method was not called since it's a duplicate
        await _bankTransactionRepository.DidNotReceive().AddBankTransactionMappingAsync(Arg.Any<BankTransactionMapping>());
    }

    /// <summary>
    /// Tests that when no duplicate bank transaction mapping is detected, the service successfully adds the new mapping using the repository's 
    /// AddBankTransactionMappingAsync method.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddBankTransactionMappingAsync_AddsMapping_WhenNoDuplicateExists()
    {
        // Arrange
        var mapping = new BankTransactionMapping
        {
            Id = Guid.NewGuid(),
            SearchTerm = "Unique Mapping",
            DescriptionTemplate = "This is a unique mapping.",
            ReferenceTemplate = "TEST",
            TargetGeneralLedgerAccountId = 5000,
            Priority = 1,
            MatchStrategy = BankTransactionMatchStrategy.Exact
        };
        
        // Set up the repository to return false for duplicate check and to return the mapping when adding
        _bankTransactionRepository.IsBankTransactionMappingDuplicateAsync(mapping).Returns(false);
        // Set up the repository to return the mapping when AddBankTransactionMappingAsync is called
        _bankTransactionRepository.AddBankTransactionMappingAsync(mapping).Returns(mapping);

        // Act
        await _bankTransactionService.AddBankTransactionMappingAsync(mapping);

        // Assert
        await _bankTransactionRepository.Received(1).AddBankTransactionMappingAsync(mapping);        
    }

    /// <summary>
    /// Tests that when a null bank transaction mapping is passed to the service, it throws an ArgumentNullException with the appropriate error message, 
    /// and that the repository's IsBankTransactionMappingDuplicateAsync and AddBankTransactionMappingAsync methods are not called since the input is invalid.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddBankTransactionMappingAsync_ThrowsArgumentNullException_WhenMappingIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _bankTransactionService.AddBankTransactionMappingAsync(null!));
        Assert.Equal("The bank transaction mapping cannot be null. (Parameter 'mapping')", exception.Message);

        // Verify that the IsBankTransactionMappingDuplicateAsync method was not called since the mapping is null
        await _bankTransactionRepository.DidNotReceive().IsBankTransactionMappingDuplicateAsync(Arg.Any<BankTransactionMapping>());
        // Verify that the AddBankTransactionMappingAsync method was not called since the mapping is null
        await _bankTransactionRepository.DidNotReceive().AddBankTransactionMappingAsync(Arg.Any<BankTransactionMapping>());
    }
}

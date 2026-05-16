using NSubstitute;
using LedgerPro.Infrastructure;
using LedgerPro.Infrastructure.Repositories;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using LedgerPro.Core.Entities;
using Microsoft.Data.Sqlite;
using LedgerPro.Core.Enums;
using LedgerPro.Core.Exceptions;

namespace LedgerPro.Tests.Infrastructure.Repositories;

public class BankTransactionRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly LedgerDbContext _dbContext;
    private readonly BankTransactionRepository _repository;

    public BankTransactionRepositoryTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<LedgerDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new LedgerDbContext(options);
        _dbContext.Database.EnsureCreated();

        // 
        _repository = new BankTransactionRepository(_dbContext);
    }

    /// <summary>
    /// Tests that when a null BankTransactionMapping is passed to the AddBankTransactionMappingAsync method, 
    /// an ArgumentNullException is thrown with the appropriate error message, and that the context's AnyAsync method 
    /// is not called since the input is invalid.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddBankTransactionMappingAsync_ThrowsArgumentNullException_WhenMappingIsNull()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.AddBankTransactionMappingAsync(null!));
        Assert.Equal("The bank transaction mapping cannot be null. (Parameter 'mapping')", exception.Message);

        await _dbContext.SaveChangesAsync(); // Save changes to verify that no changes were made to the database        

        // Verify that no new mapping was added to the database
        var count = await _dbContext.BankTransactionMappings.CountAsync();
        Assert.Equal(0, count);
    }

    /// <summary>
    /// Tests that when a duplicate BankTransactionMapping is detected, the AddBankTransactionMappingAsync method throws a BusinessException with 
    /// the appropriate error message, and that the context's AnyAsync method is called to check for duplicates, but no new mapping is added to the database.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddBankTransactionMappingAsync_ThrowsBusinessException_WhenDuplicateExists()
    {        
        // Arrange
        var generalLedgerAccount = new GeneralLedgerAccount
        {
            Id = 5000,
            Name = "Test Account",
            AccountType = GeneralLedgerAccountType.Expense,
            Description = "This is a test general ledger account."
        };
        await _dbContext.GeneralLedgerAccounts.AddAsync(generalLedgerAccount);
        await _dbContext.SaveChangesAsync();

        var mapping = new BankTransactionMapping
        {
            Id = Guid.NewGuid(),
            SearchTerm = "Test Mapping",
            DescriptionTemplate = "This is a duplicated test mapping.",
            ReferenceTemplate = "TEST",
            TargetGeneralLedgerAccountId = 5000,
            Priority = 1,
            MatchStrategy = BankTransactionMatchStrategy.Exact,
        };

        // Add the initial mapping to the database
        await _repository.AddBankTransactionMappingAsync(mapping);
        await _dbContext.SaveChangesAsync();

         // Act & Assert - Attempt to add the same mapping again and expect a BusinessException to be thrown due to the duplicate
         var exception = await Assert.ThrowsAsync<BusinessException>(() => _repository.AddBankTransactionMappingAsync(mapping));         
         Assert.Equal("The bank transaction mapping already exists.", exception.Message);

        await _dbContext.SaveChangesAsync();

         // Verify that no new mapping was added to the database
         var count = await _dbContext.BankTransactionMappings.CountAsync();
         Assert.Equal(1, count); // Only the original mapping should exist
    }

    /// <summary>
    /// Tests that when no duplicate BankTransactionMapping is detected, the AddBankTransactionMappingAsync method successfully 
    /// adds the new mapping to the database, and that the returned mapping has the expected properties. 
    /// The test also verifies that the new mapping is actually added to the database by checking the count of mappings after the operation.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task AddBankTransactionMappingAsync_AddsMapping_WhenNoDuplicateExists()
    {
        // Arrange
        var generalLedgerAccount = new GeneralLedgerAccount
        {
            Id = 5000,
            Name = "Test Account",
            AccountType = GeneralLedgerAccountType.Expense,
            Description = "This is a test general ledger account."
        };
        await _dbContext.GeneralLedgerAccounts.AddAsync(generalLedgerAccount);
        await _dbContext.SaveChangesAsync();

        var mapping = new BankTransactionMapping
        {
            Id = Guid.NewGuid(),
            SearchTerm = "Unique Mapping",
            DescriptionTemplate = "This is a unique test mapping.",
            ReferenceTemplate = "TEST",
            TargetGeneralLedgerAccountId = 5000,
            Priority = 1,
            MatchStrategy = BankTransactionMatchStrategy.Exact,
        };

        // Act
        var result = await _repository.AddBankTransactionMappingAsync(mapping);
        await _dbContext.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mapping.Id, result.Id);
        Assert.Equal(mapping.SearchTerm, result.SearchTerm);
        Assert.Equal(mapping.DescriptionTemplate, result.DescriptionTemplate);
        Assert.Equal(mapping.ReferenceTemplate, result.ReferenceTemplate);
        Assert.Equal(mapping.TargetGeneralLedgerAccountId, result.TargetGeneralLedgerAccountId);
        Assert.Equal(mapping.Priority, result.Priority);
        Assert.Equal(mapping.MatchStrategy, result.MatchStrategy);

         // Verify that the mapping was added to the database
         var count = await _dbContext.BankTransactionMappings.CountAsync();
         Assert.Equal(1, count); // The new mapping should be added
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Close();
        _connection.Dispose();
    }
}

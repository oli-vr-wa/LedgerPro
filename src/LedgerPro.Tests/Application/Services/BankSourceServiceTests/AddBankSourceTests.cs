using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LedgerPro.Application.DTOs.BankSource;
using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using LedgerPro.Core.Exceptions;
using NSubstitute;

namespace LedgerPro.Tests.Application.Services.BankSourceServiceTests;

public class AddBankSourceTests : BankSourceServiceTestsBase
{    
    [Fact]
    public async Task AddBankSourceAsync_ReturnsNewBankSourceId_WhenRequestIsValid()
    {
        // Arrange
        var request = new AddBankSourceRequest(
            AccountName: "Test Account",
            AccountNumber: "1234567890",
            BankName: "Test Bank",
            BankType: BankType.NAB
        );

        var glAccountsBankSource = new List<GeneralLedgerAccount>
        {
            new GeneralLedgerAccount { Id = 1000, Name = "Existing Bank Source 1", AccountType = GeneralLedgerAccountType.Asset },
            new GeneralLedgerAccount { Id = 1001, Name = "Existing Bank Source 2", AccountType = GeneralLedgerAccountType.Asset }
        };

        _generalLedgerRepository.GetGeneralLedgerAccountsByRangeAsync(1000, 1010).Returns(glAccountsBankSource);
        _bankSourceRepository.AddBankSourceAsync(Arg.Any<BankSource>()).Returns(Task.CompletedTask);

        // Act
        var result = await _bankSourceService.AddBankSourceAsync(request);

        // Assert
        Assert.IsType<Guid>(result);
        Assert.NotEqual(Guid.Empty, result); 
        await _bankSourceRepository.Received(1).AddBankSourceAsync(Arg.Is((BankSource bs) =>
            bs.AccountName == request.AccountName &&
            bs.AccountNumber == request.AccountNumber &&
            bs.BankName == request.BankName &&
            bs.BankType == request.BankType &&
            bs.GeneralLedgerAccountId == 1002 // Next available GL account ID after 1000 and 1001
        ));
        
        // Verify that the repository methods were called once
        await _generalLedgerRepository.Received(1).GetGeneralLedgerAccountsByRangeAsync(1000, 1010);
        await _bankSourceRepository.Received(1).AddBankSourceAsync(Arg.Any<BankSource>());
    }

    [Fact]
    public async Task AddBankSourceAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        AddBankSourceRequest request = null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _bankSourceService.AddBankSourceAsync(request));
        // Verify that the repository methods were not called
        await _generalLedgerRepository.DidNotReceive().GetGeneralLedgerAccountsByRangeAsync(Arg.Any<int>(), Arg.Any<int>());
        await _bankSourceRepository.DidNotReceive().AddBankSourceAsync(Arg.Any<BankSource>());        
    }

    [Fact]
    public async Task AddBankSourceAsync_ThrowsInvalidOperationException_WhenMaximumBankSourcesReached()
    {
        // Arrange
        var request = new AddBankSourceRequest(
            AccountName: "Test Account",
            AccountNumber: "1234567890",
            BankName: "Test Bank",
            BankType: BankType.NAB
        );

        var glAccountsBankSource = Enumerable.Range(1000, 11)
            .Select(i => new GeneralLedgerAccount { 
                Id = i, 
                Name = $"Existing Bank Source {i}", 
                AccountType = GeneralLedgerAccountType.Asset }).ToList();

        _generalLedgerRepository.GetGeneralLedgerAccountsByRangeAsync(1000, 1010).Returns(glAccountsBankSource);

        // Act & Assert
        await Assert.ThrowsAsync<BusinessException>(() => _bankSourceService.AddBankSourceAsync(request));
        // Verify that the repository methods were called once
        await _generalLedgerRepository.Received(1).GetGeneralLedgerAccountsByRangeAsync(1000, 1010);
        await _bankSourceRepository.DidNotReceive().AddBankSourceAsync(Arg.Any<BankSource>());        
    }
}

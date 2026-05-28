using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LedgerPro.Application.DTOs.BankSource;
using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;

namespace LedgerPro.Application.Services;

public class BankSourceService(
    IBankSourceRepository bankSourceRepository,
    IGeneralLedgerRepository generalLedgerRepository) : IBankSourceService
{
    private readonly IBankSourceRepository _bankSourceRepository = bankSourceRepository;
    private readonly IGeneralLedgerRepository _generalLedgerRepository = generalLedgerRepository;

    /// <summary>
    /// Adds a new bank source to the system. This involves creating a new bank source record and an associated 
    /// general ledger account for tracking transactions related to this bank source. The method validates the input request, 
    /// generates the next available general ledger account ID within a specified range for bank sources, and ensures that 
    /// the maximum number of bank sources has not been exceeded before adding the new bank source to the repository.
    /// </summary>
    /// <param name="request">The request object containing the details of the bank source to be added.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the request is null.</exception>
    /// <exception cref="ArgumentException">Thrown when any required property of the request is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the maximum number of bank sources has been reached.</exception>
    public async Task<Guid> AddBankSourceAsync(AddBankSourceRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request), "The add bank source request cannot be null.");
        if (string.IsNullOrWhiteSpace(request.AccountName))
            throw new ArgumentException("The account name is required.", nameof(request.AccountName));
        if (string.IsNullOrWhiteSpace(request.AccountNumber))
            throw new ArgumentException("The account number is required.", nameof(request.AccountNumber));
        if (string.IsNullOrWhiteSpace(request.BankName))
            throw new ArgumentException("The bank name is required.", nameof(request.BankName));        

        var glAccountsBankSource = await _generalLedgerRepository.GetGeneralLedgerAccountsByRangeAsync(1000, 1010);

        // Generate the next available general ledger account ID for the new bank source. 
        // This assumes that bank sources are assigned GL account IDs in the range of 1000-1010.
        int nextGlAccountId = glAccountsBankSource.Any() ? glAccountsBankSource.Max(a => a.Id) + 1 : 1000;

        // Check if the next available GL account ID exceeds the maximum allowed for bank sources.
        if (nextGlAccountId > 1010)
            throw new InvalidOperationException("The maximum number of bank sources has been reached. Cannot add more bank sources.");     

        var newGlAccount = new GeneralLedgerAccount
        {
            Id = nextGlAccountId,
            Name = $"{request.BankName} - {request.AccountNumber.Substring(request.AccountNumber.Length - 4)}", // Use the last 4 digits of the account number for the GL account name for easier identification
            AccountType = GeneralLedgerAccountType.Asset,
            Description = $"Bank source for {request.BankName} account ending in {request.AccountNumber.Substring(request.AccountNumber.Length - 4)}",
        };

        var bankSource = new BankSource
        {
            Id = Guid.NewGuid(),
            AccountName = request.AccountName,
            AccountNumber = request.AccountNumber,
            BankName = request.BankName,
            BankType = request.BankType,
            GeneralLedgerAccountId = newGlAccount.Id,
            GeneralLedgerAccount = newGlAccount,
        };

        await _bankSourceRepository.AddBankSourceAsync(bankSource);

        return bankSource.Id;
    }
}

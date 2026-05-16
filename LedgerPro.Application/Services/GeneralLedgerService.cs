using LedgerPro.Core.Common;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Exceptions;
using LedgerPro.Core.Interfaces;

namespace LedgerPro.Application.Services;

/// <summary>
/// Service responsible for handling business logic related to the general ledger, such as adding new accounts.
/// This service interacts with the IGeneralLedgerRepository to perform data access operations. 
/// The service ensures that business rules are enforced, such as validating that a general ledger account ID is not already in use before adding a new account.
/// </summary>
/// <param name="generalLedgerRepository">The repository used to access general ledger data.</param>
public class GeneralLedgerService(IGeneralLedgerRepository generalLedgerRepository) : IGeneralLedgerService
{
    private readonly IGeneralLedgerRepository _generalLedgerRepository = generalLedgerRepository;

    /// <summary>
    /// Adds a new general ledger account to the system. Validates that the account ID is not already in use before adding.
    /// If the account ID is already in use, throws a BusinessException with an appropriate error message. 
    /// If the account is added successfully, the operation completes without returning a value.
    /// </summary>
    /// <param name="account">The general ledger account to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddGeneralLedgerAccountAsync(GeneralLedgerAccount account)
    {
        if (account == null)
            throw new ArgumentNullException(nameof(account), "Account cannot be null.");

        bool isInUse = await _generalLedgerRepository.IsGeneralLedgerAccountIdInUseAsync(account.Id);

        if (isInUse)
            throw new BusinessException($"General ledger account with ID {account.Id} is already in use and cannot be added.");

        await _generalLedgerRepository.AddGeneralLedgerAccountAsync(account);
    }
}

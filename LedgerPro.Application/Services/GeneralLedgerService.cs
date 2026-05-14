using LedgerPro.Core.Common;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Interfaces;

namespace LedgerPro.Application.Services;

/// <summary>
/// Service responsible for handling business logic related to the general ledger, such as adding new accounts.
/// This service interacts with the IGeneralLedgerRepository to perform data access operations and uses the IUnitOfWork to manage transactions. 
/// The service ensures that business rules are enforced, such as validating that a general ledger account ID is not already in use before adding a new account.
/// </summary>
/// <param name="generalLedgerRepository">The repository used to access general ledger data.</param>
/// <param name="unitOfWork">The unit of work used to manage transactions.</param>
public class GeneralLedgerService(IGeneralLedgerRepository generalLedgerRepository, IUnitOfWork unitOfWork) : IGeneralLedgerService
{
    private readonly IGeneralLedgerRepository _generalLedgerRepository = generalLedgerRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <summary>
    /// Adds a new general ledger account to the system. Validates that the account ID is not already in use before adding.
    /// If the account ID is already in use, returns a failure result with an appropriate error message. 
    /// If the account is added successfully, commits the transaction and returns a success result containing the added account.
    /// </summary>
    /// <param name="account">The general ledger account to add.</param>
    /// <returns>A result indicating success or failure, containing the added account if successful.</returns>
    public async Task<Result<GeneralLedgerAccount>> AddGeneralLedgerAccountAsync(GeneralLedgerAccount account)
    {
        bool isInUse = await _generalLedgerRepository.IsGeneralLedgerAccountIdInUseAsync(account.Id);
        if (isInUse)
            return Result<GeneralLedgerAccount>.Failure($"General ledger account with ID {account.Id} is already in use and cannot be added.");

        await _generalLedgerRepository.AddGeneralLedgerAccountAsync(account);
        await _unitOfWork.CommitAsync();
        return Result<GeneralLedgerAccount>.Success(account);
    }
}

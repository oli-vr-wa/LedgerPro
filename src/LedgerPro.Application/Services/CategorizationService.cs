using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LedgerPro.Application.DTOs.BankStatement;
using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Core.Common;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using LedgerPro.Core.Exceptions;

namespace LedgerPro.Application.Services;

public class CategorizationService(IGeneralLedgerRepository generalLedgerRepository, IBankTransactionRepository bankTransactionRepository) : ICategorizationService
{
    private readonly IGeneralLedgerRepository _generalLedgerRepository = generalLedgerRepository;
    private readonly IBankTransactionRepository _bankTransactionRepository = bankTransactionRepository;

    /// <summary>
    /// Categorizes a bank transaction by creating general ledger items based on the provided categorization details.
    /// </summary>
    /// <param name="categorizeDto">The DTO containing the categorization details for the bank transaction.</param>
    /// <returns>A result indicating whether the categorization was successful.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the categorize DTO is null.</exception>
    /// <exception cref="BusinessException">Thrown when there is an issue with the business logic, such as a missing general ledger account.</exception>
    public async Task<Result<bool>> CategorizeBankTransactionAsync(BankTransactionCategorize categorizeDto)
    {
        if (categorizeDto == null)
            throw new ArgumentNullException(nameof(categorizeDto), "Categorize DTO cannot be null.");

        var bankTransaction = await _bankTransactionRepository.GetBankTransactionWithGlItemsByIdAsync(categorizeDto.BankTransactionId);

        if (bankTransaction == null)
            return Result<bool>.Failure($"Bank transaction with ID {categorizeDto.BankTransactionId} not found.");   
        // Check that the bank transaction is not already reconciled.
        if (bankTransaction.Status == BankTransactionStatus.Reconciled)
            return Result<bool>.Failure($"Bank transaction with ID {categorizeDto.BankTransactionId} is already reconciled and cannot be categorized.");                 
        
        if (bankTransaction.Amount != categorizeDto.CategorizeItems.Sum(i => i.Amount))
            return Result<bool>.Failure($"The sum of the amounts in the categorize items must equal the amount of the bank transaction. Bank transaction amount: {bankTransaction.Amount}, sum of categorize items: {categorizeDto.CategorizeItems.Sum(i => i.Amount)}.");

        // Clear current general ledger items
        bankTransaction.GeneralLedgerItems.Clear();

        var accounts = await _generalLedgerRepository.GetGeneralLedgerAccountsAsync();    

        // If there are multiple items to categorize, create additional general ledger items for each one
        var glItemsToCreate = new List<GeneralLedgerItem>();
        foreach (var item in categorizeDto.CategorizeItems)
        {
            // Check if the gl account exists for the provided GeneralLedgerAccountId, if not throw a BusinessException to indicate the issue with the provided account ID.
            if (!accounts.Any(a => a.Id == item.GeneralLedgerAccountId))
                throw new BusinessException($"General ledger account with ID {item.GeneralLedgerAccountId} not found.");

            bankTransaction.AddLedgerEntry(item.GeneralLedgerAccountId, item.Amount, item.Description, item.Reference);
        }

        // Set Bank Transaction status to Categorized after successfully creating the general ledger items
        bankTransaction.Status = BankTransactionStatus.Categorized;

        return Result<bool>.Success(true);
    }
}

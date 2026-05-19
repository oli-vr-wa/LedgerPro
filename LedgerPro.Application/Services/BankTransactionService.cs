using LedgerPro.Core.Entities;
using LedgerPro.Core.Exceptions;
using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Application.DTOs.BankStatement;
using LedgerPro.Core.Enums;

namespace LedgerPro.Application.Services;

public class BankTransactionService(
    IBankTransactionRepository bankTransactionRepository) : IBankTransactionService
{
    private readonly IBankTransactionRepository _bankTransactionRepository = bankTransactionRepository;

    /// <summary>
    /// Adds a new bank transaction mapping to the database. 
    /// Before adding the mapping, it checks if a mapping with the same criteria already exists to prevent duplicates. 
    /// If a duplicate is found, a BusinessException is thrown. If no duplicate exists, the new mapping is added to the database using the IBankTransactionRepository.
    /// </summary>
    /// <param name="mapping">The BankTransactionMapping entity to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="BusinessException">Thrown if a duplicate bank transaction mapping is found.</exception>
    public async Task AddBankTransactionMappingAsync(BankTransactionMapping mapping)
    {
        if (mapping == null)
            throw new ArgumentNullException(nameof(mapping), "The bank transaction mapping cannot be null.");

        bool isDuplicate = await _bankTransactionRepository.IsBankTransactionMappingDuplicateAsync(mapping);

        if (isDuplicate)        
            throw new BusinessException("The bank transaction mapping already exists.");
        
        await _bankTransactionRepository.AddBankTransactionMappingAsync(mapping);
    }

    /// <summary>
    /// Reconciles a bank transaction by creating general ledger items based on the provided split general ledger items in the request. 
    /// The method first validates the request to ensure that it is not null, contains at least one split general ledger item, and 
    /// that the total amount of the split items matches the bank transaction amount.
    /// </summary>
    /// <param name="request">The request containing the bank transaction ID and split general ledger items for reconciliation.</param>
    /// <returns>The number of general ledger items added.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the request is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the request contains no split general ledger items or if the total amount 
    /// does not match the bank transaction amount.</exception>
    public async Task<int> ReconcileBankTransactionAsync(ReconcileBankTransactionRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request), "The reconcile bank transaction request cannot be null.");

        // Check if there are any split general ledger items provided in the request. 
        // If not, throw an exception since at least one item is required for reconciliation.
        if (request.SplitGeneralLedgerItems == null || request.SplitGeneralLedgerItems.Count == 0)
            throw new ArgumentException("At least one split general ledger item is required for reconciliation.");

        // Retrieve the bank transaction to be reconciled using the provided BankTransactionId.
        var bankTransaction = await _bankTransactionRepository.GetBankTransactionByIdAsync(request.BankTransactionId);                

        // Verify that the total amount of the split general ledger items matches the amount of the bank transaction.
        if (request.SplitGeneralLedgerItems.Sum(i => i.Amount) != bankTransaction.Amount)
            throw new ArgumentException("The total amount of the split general ledger items must equal the amount of the bank transaction.");

        // Create a list of GeneralLedgerItem entities based on the split general ledger items provided in the request.
        var generalLedgerItemsToAdd = request.SplitGeneralLedgerItems.Select(i => new GeneralLedgerItem
        {
            Id = Guid.NewGuid(),
            BankTransactionId = bankTransaction.Id,
            GeneralLedgerAccountId = i.GeneralLedgerAccountId,
            Description = i.Description,
             Amount = i.Amount,
             Reference = i.Reference,
             TransactionDate = bankTransaction.TransactionDate,
             IsReconciled = true,
             Side = bankTransaction.Amount < 0 ? TransactionSide.Debit : TransactionSide.Credit
        }).ToList();
    
        int glItemsAdded = await _bankTransactionRepository.ReconcileBankTransactionAsync(bankTransaction, generalLedgerItemsToAdd);
        
        return glItemsAdded;
    }
}

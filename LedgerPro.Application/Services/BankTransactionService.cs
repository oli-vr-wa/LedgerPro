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
    /// Confirms the reconciliation of a categorized bank transaction by creating a general ledger item for the bank transaction and updating its status to reconciled.
    /// The method first retrieves the bank transaction by its ID and checks if it is in a categorized status. It also verifies that there is at least 
    /// one general ledger item associated with the bank transaction and that no bank transaction general ledger item already exists.
    /// If the bank transaction is valid for confirmation, a new general ledger item is created for the bank transaction, and the reconciliation is 
    /// confirmed by calling the corresponding method in the IBankTransactionRepository.
    /// </summary>
    /// <param name="bankTransactionId">The ID of the bank transaction to confirm reconciliation for.</param>
    /// <returns>A task representing the asynchronous operation, with the result being the number of affected rows.</returns>
    /// <exception cref="ArgumentException">Thrown if the bank transaction ID is empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the bank transaction is not categorized or other validation fails.</exception>
    public Task<int> ConfirmReconcileCategorizedBankTransactionAsync(Guid bankTransactionId)
    {
        if (bankTransactionId == Guid.Empty)
            throw new ArgumentException("The bank transaction ID cannot be empty.", nameof(bankTransactionId));
        
        var bankTransaction = _bankTransactionRepository.GetBankTransactionByIdAsync(bankTransactionId).Result ?? 
            throw new InvalidOperationException("The bank transaction to confirm reconcile was not found.");

        if (bankTransaction.Status != BankTransactionStatus.Categorized)
            throw new InvalidOperationException("Only categorized bank transactions can be confirmed for reconciliation.");
        
        // Check that there is at least one general ledger item associated and no bank transaction general ledger item exists.
        if (bankTransaction.GeneralLedgerItems == null || bankTransaction.GeneralLedgerItems.Count == 0)
            throw new InvalidOperationException("At least one general ledger item is required to confirm reconciliation of a categorized bank transaction.");

        if (bankTransaction.GeneralLedgerItems.Any(i => i.GeneralLedgerAccountId == bankTransaction.BankSource.GeneralLedgerAccountId))
            throw new InvalidOperationException("A bank transaction general ledger item already exists.");

        var bankTransactionGlItem = new GeneralLedgerItem
        {
            Id = Guid.NewGuid(),
            BankTransactionId = bankTransaction.Id,
            GeneralLedgerAccountId = bankTransaction.BankSource.GeneralLedgerAccountId,
            Description = $"{bankTransaction.Description} - Reconciled Bank Transaction",
            Amount = bankTransaction.Amount,
            Reference = GenerateBankTransactionGlItemReference(bankTransaction),
            TransactionDate = bankTransaction.TransactionDate,
            IsReconciled = true,
            Side = bankTransaction.Amount < 0 ? TransactionSide.Debit : TransactionSide.Credit
        };

        return _bankTransactionRepository.ConfirmReconcileCategorizedBankTransactionAsync(bankTransaction, bankTransactionGlItem);
    }

    /// <summary>
    /// Reconciles a bank transaction by creating general ledger items based on the provided split general ledger items in the request. 
    /// The method first validates the request to ensure that it is not null, contains at least one split general ledger item, and 
    /// that the total amount of the split items matches the bank transaction amount.
    /// </summary>
    /// <param name="request">The request containing the bank transaction ID and split general ledger items for reconciliation.</param>
    /// <returns>The number of general ledger items added.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the request is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the request contains no split general ledger items.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the total amount of the split general ledger items does not match the bank transaction amount.</exception>
    public async Task<int> ReconcileBankTransactionAsync(ReconcileBankTransactionRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request), "The reconcile bank transaction request cannot be null.");

        // Check if there are any split general ledger items provided in the request. 
        // If not, throw an exception since at least one item is required for reconciliation.
        if (request.SplitGeneralLedgerItems == null || request.SplitGeneralLedgerItems.Count == 0)
            throw new ArgumentException("At least one split general ledger item is required for reconciliation.");

        // Retrieve the bank transaction to be reconciled using the provided BankTransactionId.
        var bankTransaction = await _bankTransactionRepository.GetBankTransactionByIdAsync(request.BankTransactionId) ?? 
            throw new InvalidOperationException("The bank transaction to reconcile was not found.");

        if (bankTransaction.Status == BankTransactionStatus.Reconciled)
            throw new InvalidOperationException("The bank transaction has already been reconciled.");

        // Verify that the total amount of the split general ledger items matches the amount of the bank transaction.
        if (request.SplitGeneralLedgerItems.Sum(i => i.Amount) != bankTransaction.Amount)
            throw new InvalidOperationException("The total amount of the split general ledger items must equal the amount of the bank transaction.");

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

        // Create bank transaction general ledger item.
        var bankTransactionGeneralLedgerItem = new GeneralLedgerItem
        {
            Id = Guid.NewGuid(),
            BankTransactionId = bankTransaction.Id,
            GeneralLedgerAccountId = bankTransaction.BankSource.GeneralLedgerAccountId,
            Description = $"{bankTransaction.Description} - Reconciled Bank Transaction",
            Amount = bankTransaction.Amount,
            Reference = GenerateBankTransactionGlItemReference(bankTransaction),
            TransactionDate = bankTransaction.TransactionDate,
            IsReconciled = true,
            Side = bankTransaction.Amount < 0 ? TransactionSide.Debit : TransactionSide.Credit
        };
        generalLedgerItemsToAdd.Add(bankTransactionGeneralLedgerItem);
    
        int glItemsAdded = await _bankTransactionRepository.ReconcileBankTransactionAsync(bankTransaction, generalLedgerItemsToAdd);
        
        return glItemsAdded;
    }

    /// <summary>
    /// Unreconciles a bank transaction by removing the associated general ledger items and updating the bank transaction status.
    /// </summary>
    /// <param name="bankTransactionId">The ID of the bank transaction to unreconcile.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the bank transaction is not found or is not reconciled.</exception>
    public async Task UnreconcileBankTransactionAsync(Guid bankTransactionId)
    {
        var bankTransaction = await _bankTransactionRepository.GetBankTransactionByIdAsync(bankTransactionId) ?? 
            throw new InvalidOperationException("The bank transaction to unreconcile was not found.");

        if (bankTransaction.Status != BankTransactionStatus.Reconciled)
            throw new InvalidOperationException("The bank transaction is not reconciled and cannot be unreconciled.");

        await _bankTransactionRepository.UnreconcileBankTransactionAsync(bankTransaction);
    }

    /// <summary>
    /// Generates a reference string for a bank transaction general ledger item based on the transaction date and a portion of the transaction ID.
    /// </summary>
    /// <param name="bankTransaction">The bank transaction for which to generate the reference.</param>
    /// <returns>A reference string for the bank transaction general ledger item.</returns>
    private string GenerateBankTransactionGlItemReference(BankTransaction bankTransaction)
    {
        return $"REC-{bankTransaction.TransactionDate:yyMMdd}-{bankTransaction.Id.ToString().Substring(0, 4)}";
    }    
}

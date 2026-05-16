using System.Runtime.CompilerServices;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Exceptions;
using LedgerPro.Core.Interfaces;

namespace LedgerPro.Application.Services;

public class BankTransactionService(IBankTransactionRepository bankTransactionRepository) : IBankTransactionService
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
        bool isDuplicate = await _bankTransactionRepository.IsBankTransactionMappingDuplicateAsync(mapping);

        if (isDuplicate)        
            throw new BusinessException("The bank transaction mapping already exists.");
        
        await _bankTransactionRepository.AddBankTransactionMappingAsync(mapping);
    }
}

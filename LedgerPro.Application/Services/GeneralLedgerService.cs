using LedgerPro.Application.DTOs.Reports;
using LedgerPro.Application.Extensions;
using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Application.Validation.BankTransaction;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using LedgerPro.Core.Exceptions;

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

    /// <summary>
    /// Retrieves a summary of accounts for a specified financial year, including total debits, total credits, and calculated balance for each account.
    /// If the financialYearEnding parameter is not provided, it defaults to the current financial year based on the current date. 
    /// The method applies accounting rules to calculate the balance for each account based on its type (Asset, Liability, Equity, Revenue, Expense).
    /// The result is returned as a list of IAccountSummaryRowDto objects, which contain the account ID, name, type, total debits, total credits, 
    /// and calculated balance for each account.  
    /// </summary>
    /// <param name="financialYearEnding">The ending year of the financial year for which to retrieve the account summary. 
    /// If not provided, defaults to the current financial year.</param>
    /// <returns>A task representing the asynchronous operation, containing a list of account summary rows.</returns>
    public async Task<List<AccountSummaryRowDto>> GetFinancialYearAccountsSummaryAsync(int? financialYearEnding = null)
    {
        if (financialYearEnding == null)
        {
            var currentDate = DateTime.Now;
            // if the current date is between July and December, we consider the financial year to be the next calendar year (e.g., if it's 2024-08-01, the financial year is 2025)
            if (currentDate.Month >= 7)
                financialYearEnding = currentDate.Year + 1;
            else
                financialYearEnding = currentDate.Year; // Default to current year if not provided
        }
        else 
        {
            await ValidateFinancialYearEndingAsync(financialYearEnding.Value);
        }

        DateTime financialYearStart = financialYearEnding.Value.GetFinancialYearStart(); // Start of the financial year (July 1st of the previous year)
        DateTime financialYearEnd = financialYearEnding.Value.GetFinancialYearEnd(); // End of the financial year (June 30th of the current year)

        var accountsTotals = await _generalLedgerRepository.GetGlAccountFinancialTotalAsync(financialYearStart, financialYearEnd);

        // Apply accounting rules to calculate the balance for each account based on its type
        var accountSummaries = accountsTotals.Select(at =>
        {
            decimal balance = at.AccountType switch
            {
                GeneralLedgerAccountType.Asset => at.TotalDebits - at.TotalCredits,
                GeneralLedgerAccountType.Expense => at.TotalDebits - at.TotalCredits,
                GeneralLedgerAccountType.Liability => at.TotalCredits - at.TotalDebits,
                GeneralLedgerAccountType.Equity => at.TotalCredits - at.TotalDebits,
                GeneralLedgerAccountType.Revenue => at.TotalCredits - at.TotalDebits,
                _ => 0m
            };

            return new AccountSummaryRowDto
            {
                AccountId = at.AccountId,
                AccountName = at.AccountName,
                AccountType = at.AccountType,
                TotalDebits = at.TotalDebits,
                TotalCredits = at.TotalCredits,
                Balance = balance
            };
        })
        .ToList();

        return accountSummaries;
    } 

    /// <summary>
    /// Retrieves a summary of financial metrics for the dashboard, including total income, total expenses, assets, liabilities, 
    /// and the count of unreconciled transactions for a specified financial year.
    /// </summary>
    /// <param name="financialYearEnding">The ending year of the financial year for which to retrieve the summary.</param>
    /// <returns>A <see cref="DashboardSummaryDto"/> containing the financial metrics for the specified financial year.</returns>
    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(int financialYearEnding)
    {
        // Validate the financialYearEnding parameter to ensure it falls within a reasonable range (e.g., between 1900 and 2100)
        await ValidateFinancialYearEndingAsync(financialYearEnding);

        DateTime fromDate = financialYearEnding.GetFinancialYearStart(); // Start of the financial year (July 1st of the previous year)
        DateTime toDate = financialYearEnding.GetFinancialYearEnd(); // End of the financial year (June 30th of the current year)

        var accountTypeMapping = new Dictionary<GeneralLedgerAccountType, GeneralLedgerAccountType>
        {
            { GeneralLedgerAccountType.Asset, GeneralLedgerAccountType.Asset },
            { GeneralLedgerAccountType.Liability, GeneralLedgerAccountType.Liability },
            { GeneralLedgerAccountType.Revenue, GeneralLedgerAccountType.Revenue },
            { GeneralLedgerAccountType.Expense, GeneralLedgerAccountType.Expense }
        };

        var ledgerItems = await _generalLedgerRepository.GetDashboardSummaryGeneralLedgerItemsAsync(fromDate, toDate, accountTypeMapping);
        int unreconciledTransactionsCount = await _generalLedgerRepository.GetUnreconciledTransactionsCountAsync(fromDate, toDate);

        var summary = new DashboardSummaryDto
        {
            TotalIncome = ledgerItems.Where(item => item.AccountType == GeneralLedgerAccountType.Revenue)
                                     .Sum(item => item.Side == TransactionSide.Credit ? item.Amount : -item.Amount),
            TotalExpense = ledgerItems.Where(item => item.AccountType == GeneralLedgerAccountType.Expense)
                                      .Sum(item => item.Side == TransactionSide.Debit ? item.Amount : -item.Amount),
            Assets = ledgerItems.Where(item => item.AccountType == GeneralLedgerAccountType.Asset)
                                .Sum(item => item.Side == TransactionSide.Debit ? item.Amount : -item.Amount),
            Liabilities = ledgerItems.Where(item => item.AccountType == GeneralLedgerAccountType.Liability)
                                     .Sum(item => item.Side == TransactionSide.Credit ? item.Amount : -item.Amount),
            UnreconciledTransactionsCount = unreconciledTransactionsCount
        };

        return summary;   
    }

    /// <summary>
    /// Validates the financialYearEnding parameter to ensure it falls within a reasonable range (e.g., between 1900 and 2100) using the GetValidFinancialYearValidator.
    /// If the parameter is invalid, throws an ArgumentException with an appropriate error message.
    /// </summary>
    /// <param name="financialYearEnding"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private async Task ValidateFinancialYearEndingAsync(int financialYearEnding)
    {
        var validator = new GetValidFinancialYearValidator();
        var validationResult = await validator.ValidateAsync(financialYearEnding);

        if (!validationResult.IsValid)
        {
            throw new ArgumentOutOfRangeException(nameof(financialYearEnding), "Invalid financial year ending parameter. " + string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }
    }
}

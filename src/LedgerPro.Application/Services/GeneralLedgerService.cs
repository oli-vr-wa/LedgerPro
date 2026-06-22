using LedgerPro.Application.DTOs.BankStatement;
using LedgerPro.Application.DTOs.Reports;
using LedgerPro.Application.Extensions;
using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Application.Validation.BankTransaction;
using LedgerPro.Core.Common;
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
public class GeneralLedgerService(IGeneralLedgerRepository generalLedgerRepository, IBankTransactionRepository bankTransactionRepository) : IGeneralLedgerService
{
    private readonly IGeneralLedgerRepository _generalLedgerRepository = generalLedgerRepository;
    private readonly IBankTransactionRepository _bankTransactionRepository = bankTransactionRepository;

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
    /// Deletes an existing general ledger account from the system. 
    /// Validates that the account is not currently in use in any general ledger items or bank transaction mappings before deletion.
    /// If the account is in use, throws a BusinessException with an appropriate error message to prevent deletion of an account that is currently being referenced.
    /// </summary>
    /// <param name="id">The ID of the general ledger account to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided ID is zero.</exception>
    /// <exception cref="BusinessException">Thrown when the general ledger account is currently in use.</exception>
    public async Task DeleteGeneralLedgerAccountAsync(int id)
    {
        if (id == 0)
            throw new ArgumentException("ID cannot be zero.", nameof(id));

        // First ensure if the account is used in any GL items or Bank Transaction Mappings, if so throw a BusinessException to prevent deletion of an account that is in use.
        bool isInUse = await _generalLedgerRepository.IsGeneralLedgerAccountIdInUseAsync(id);
        if (isInUse)
            throw new BusinessException($"General ledger account with ID {id} is currently in use and cannot be deleted.");

        await _generalLedgerRepository.DeleteGeneralLedgerAccountAsync(id);
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
            financialYearEnding = DateTime.Now.GetFinancialYearEnding().Year;        
        else 
            await ValidateFinancialYearEndingAsync(financialYearEnding.Value);

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
    /// Retrieves the monthly totals for a specified date range, including total revenues, total expenses, total liabilities,
    /// and the count of pending reconcile transactions.
    /// </summary>
    /// <param name="startDate">The start date of the range.</param>
    /// <param name="endDate">The end date of the range.</param>
    /// <returns>A <see cref="PeriodTotalsDto"/> containing the monthly totals for the specified date range.</returns>
    public async Task<PeriodTotalsDto> GetMonthlyTotalsForDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date must be less than or equal to end date.");

        var ledgerItems = await _generalLedgerRepository.GetMonthlyTotalsForDateRangeAsync(startDate, endDate);
        int pendingReconcileCount = await _bankTransactionRepository.GetPendingReconciliationCountAsync(startDate, endDate);
        var monthlyTotals = ledgerItems
            .GroupBy(item => new { item.TransactionDate.Year, item.TransactionDate.Month })
            .Select(group => new MonthlyTotalsDto
            {
                Year = group.Key.Year,
                Month = group.Key.Month,
                TotalRevenue = group.Where(item => item.AccountType == GeneralLedgerAccountType.Revenue)
                                    .Sum(item => item.Side == TransactionSide.Credit ? item.Amount : -item.Amount),
                TotalExpense = group.Where(item => item.AccountType == GeneralLedgerAccountType.Expense)
                                    .Sum(item => item.Side == TransactionSide.Debit ? item.Amount : -item.Amount),
                TotalLiability = group.Where(item => item.AccountType == GeneralLedgerAccountType.Liability)
                                        .Sum(item => item.Side == TransactionSide.Credit ? item.Amount : -item.Amount)
            })
            .ToList();

        return new PeriodTotalsDto(monthlyTotals, pendingReconcileCount);
    }

    /// <summary>
    /// Creates new general ledger items based on the provided bank transaction categorization details. 
    /// Validates the input parameters, checks for the existence of the associated bank transaction and general ledger accounts, and generates references for the new general ledger items if not provided. 
    /// If any validation fails, throws appropriate exceptions to indicate the issues with the provided data. If the general ledger items are created successfully, they are saved to the repository 
    /// and the method returns a success result.
    /// </summary>
    /// <param name="categorizeDto">The DTO containing the bank transaction categorization details.</param>
    /// <returns>A result indicating the success or failure of the operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the categorizeDto is null.</exception>
    /// <exception cref="BusinessException">Thrown when there is an issue with the provided data or the creation of general ledger items.</exception>
    public async Task<Result<bool>> CreateGeneralLedgerItemsAsync(BankTransactionCategorize categorizeDto)
    {
        if (categorizeDto == null)
            throw new ArgumentNullException(nameof(categorizeDto), "Categorize DTO cannot be null.");

        var bankTransaction = await _bankTransactionRepository.GetBankTransactionWithGlItemsByIdAsync(categorizeDto.BankTransactionId);

        if (bankTransaction == null)
            return Result<bool>.Failure($"Bank transaction with ID {categorizeDto.BankTransactionId} not found.");   
        // Check that the bank transaction is not already reconciled.
        if (bankTransaction.Status == BankTransactionStatus.Reconciled)
            return Result<bool>.Failure($"Bank transaction with ID {categorizeDto.BankTransactionId} is already reconciled and cannot be categorized.");     
        // If bank transaction is categorized, delete current general ledger items for the bank transaction before creating new ones based on the provided categorizeDto.
        if (bankTransaction.Status == BankTransactionStatus.Categorized)        
            await _generalLedgerRepository.DeleteGeneralLedgerItemsAsync(bankTransaction.GeneralLedgerItems);
        
        if (bankTransaction.Amount != categorizeDto.CategorizeItems.Sum(i => i.Amount))
            return Result<bool>.Failure($"The sum of the amounts in the categorize items must equal the amount of the bank transaction. Bank transaction amount: {bankTransaction.Amount}, sum of categorize items: {categorizeDto.CategorizeItems.Sum(i => i.Amount)}.");

        var account = await _generalLedgerRepository.GetGeneralLedgerAccountsAsync();    

        // If there are multiple items to categorize, create additional general ledger items for each one
        int itemsCount = 1;
        var glItemsToCreate = new List<GeneralLedgerItem>();
        foreach (var item in categorizeDto.CategorizeItems)
        {
            // Check if the gl account exists for the provided GeneralLedgerAccountId, if not throw a BusinessException to indicate the issue with the provided account ID.
            if (!account.Any(a => a.Id == item.GeneralLedgerAccountId))
                throw new BusinessException($"General ledger account with ID {item.GeneralLedgerAccountId} not found.");

            // If the reference is not provided in the categorizeDto, generate a reference for the general ledger item based on the bank transaction ID and transaction date.
            if (string.IsNullOrWhiteSpace(item.Reference))
            {
                item.Reference = GenerateGeneralLedgerItemReference(bankTransaction.TransactionDate, itemsCount);
                itemsCount++;   
            }            

            // Create a new general ledger item based on the bank transaction and categorization details. If the creation fails, throw a BusinessException to indicate the failure.
            var glItem = CreateGeneralLedgerItem(bankTransaction, item) ??
                throw new BusinessException($"Failed to create general ledger item for bank transaction ID {categorizeDto.BankTransactionId} and account ID {item.GeneralLedgerAccountId}.");

            glItemsToCreate.Add(glItem);
        }

        await _generalLedgerRepository.AddGeneralLedgerItemsAsync(glItemsToCreate);
        // Set Bank Transaction status to Categorized after successfully creating the general ledger items
        bankTransaction.Status = BankTransactionStatus.Categorized;

        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Creates a new general ledger item based on the provided bank transaction and categorization details.
    /// The method validates the input parameters, constructs a new GeneralLedgerItem, and saves it to the repository. The reference for the general ledger item is 
    /// generated based on the bank transaction ID and transaction date if not provided in the categorizeDto.
    /// </summary>
    /// <param name="bankTransaction">The bank transaction for which to create the general ledger item.</param>
    /// <param name="categorizeDto">The categorization details for the new general ledger item.</param>
    /// <returns>The created general ledger item.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the bank transaction or categorization details are null.</exception>
    public GeneralLedgerItem CreateGeneralLedgerItem(
        BankTransaction bankTransaction, 
        BankTransactionCategorizeItem categorizeDto)
    {
        if (bankTransaction == null)
            throw new ArgumentNullException(nameof(bankTransaction), "Bank transaction cannot be null.");
        if (categorizeDto == null)
            throw new ArgumentNullException(nameof(categorizeDto), "Categorization details cannot be null.");

        var glItem = new GeneralLedgerItem
        {
            Id = Guid.NewGuid(),
            BankTransactionId = bankTransaction.Id,
            GeneralLedgerAccountId = categorizeDto.GeneralLedgerAccountId,
            Description = categorizeDto.Description,
            Reference = categorizeDto.Reference!,
            Amount = Math.Abs(categorizeDto.Amount),
            TransactionDate = bankTransaction.TransactionDate,            
            Side = categorizeDto.Amount >= 0 ? TransactionSide.Debit : TransactionSide.Credit
        };

        return glItem;
    }

    /// <summary>
    /// Generates a unique reference string for a general ledger item based on the associated bank transaction ID and transaction date.
    /// The reference is formatted as "BT-{yyMMdd}-{N}", where "BT" indicates it's a bank transaction, "{yyMMdd}" is the transaction date in year-month-day format, 
    /// and "{N}" is a sequential number representing how many general ledger items have already been created for that bank transaction plus one.
    /// </summary>
    /// <param name="bankTransactionId">The ID of the bank transaction for which to generate the reference.</param>
    /// <param name="transactionDate">The date of the transaction for which to generate the reference.</param>
    /// <returns>A task representing the asynchronous operation, containing the generated reference string.</returns>
    public async Task<string> GenerateGeneralLedgerItemReferenceAsync(Guid bankTransactionId, DateTime transactionDate)
    {
        var count = await _generalLedgerRepository.GetGeneralLedgerItemCountForBankTransactionAsync(bankTransactionId);

        return GenerateGeneralLedgerItemReference(transactionDate, count + 1);
    }

    /// <summary>
    /// Generates a unique reference string for a general ledger item based on the transaction date and a sequence number.
    /// The reference is formatted as "BT-{yyMMdd}-{N}", where "BT" indicates it's a bank transaction, "{yyMMdd}" is the transaction date in year-month-day format, 
    /// and "{N}" is the provided sequence number.
    /// </summary>
    /// <param name="transactionDate">The date of the transaction for which to generate the reference.</param>
    /// <param name="sequenceNumber">The sequence number to include in the reference.</param>
    /// <returns>The generated reference string.</returns>
    private string GenerateGeneralLedgerItemReference(DateTime transactionDate, int sequenceNumber)
    {
        return $"BT-{transactionDate:yyMMdd}-{sequenceNumber}";
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

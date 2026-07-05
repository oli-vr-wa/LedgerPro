using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LedgerPro.Application.DTOs.Reports;
using LedgerPro.Application.Extensions;
using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Core.Enums;

namespace LedgerPro.Application.Services;

public class GeneralLedgerReportService : IGeneralLedgerReportService
{
    private readonly IGeneralLedgerRepository _generalLedgerRepository;    

    public GeneralLedgerReportService(IGeneralLedgerRepository generalLedgerRepository)
    {
        _generalLedgerRepository = generalLedgerRepository;
    }

    /// <inheritdoc />
    public async Task<List<GeneralLedgerItemRowBalanceDto>> GetGeneralLedgerItemsForFinancialYearAndAccountAsync(int financialYear, int accountId)
    {
        DateTime startDate = financialYear.GetFinancialYearStart();
        DateTime endDate = financialYear.GetFinancialYearEnd();

        var glAccount = await _generalLedgerRepository.GetAccountByIdAsync(accountId) ??
            throw new ArgumentException($"General Ledger Account with ID {accountId} does not exist.");        

        var ledgerItems = await _generalLedgerRepository.GetGeneralLedgerItemsForRangeAndAccountAsync(startDate, endDate, accountId);

        // Map the ledger items to DTOs
        var ledgerItemDtos = ledgerItems.Select(item => new GeneralLedgerItemRowBalanceDto
        {
            Id = item.Id,
            TransactionDate = item.TransactionDate,
            Reference = item.Reference,
            Description = item.Description,
            Debit = item.Side == TransactionSide.Debit ? item.Amount : 0,
            Credit = item.Side == TransactionSide.Credit ? item.Amount : 0,
            Balance = 0
        })
        .OrderBy(dto => dto.TransactionDate)        
        .ToList();

        // Calculate the running balance
        decimal runningBalance = 0;
        foreach (var dto in ledgerItemDtos)
        {
            if (glAccount.AccountType == GeneralLedgerAccountType.Asset || glAccount.AccountType == GeneralLedgerAccountType.Expense)
            {
                runningBalance += dto.Debit - dto.Credit;
            }
            else
            {
                runningBalance += dto.Credit - dto.Debit;
            }

            dto.Balance = runningBalance;
        }

        // Add an opening balance row with $0.00 for the first row.
        var openingBalanceRow = new GeneralLedgerItemRowBalanceDto
        {
            Id = Guid.Empty,
            TransactionDate = startDate,
            Reference = "Opening Balance",
            Description = "Opening Balance",
            Debit = 0,
            Credit = 0,
            Balance = 0
        };

        ledgerItemDtos.Insert(0, openingBalanceRow);        

        return ledgerItemDtos;
    }
}

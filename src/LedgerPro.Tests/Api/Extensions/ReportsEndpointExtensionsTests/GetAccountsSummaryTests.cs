using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LedgerPro.Application.DTOs.BankSource;
using LedgerPro.Application.DTOs.Reports;
using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace LedgerPro.Tests.Api.Extensions.ReportsEndpointExtensionsTests;

public class GetAccountsSummaryTests(LedgerTestFactory factory) : ReportsEndpointExtensionsTestsBase(factory)
{
    [Fact]
    public async Task GetAccountsSummary_ReturnsOkResult_WithValidFinancialYearEnding()
    {
        // Arrange
        var client = _factory.CreateClient();
        var scope = _factory.Services.CreateScope();
        var _bankSourceService = scope.ServiceProvider.GetRequiredService<IBankSourceService>();
        var _generalLedgerService = scope.ServiceProvider.GetRequiredService<IGeneralLedgerService>();
        var _generalLedgerRepository = scope.ServiceProvider.GetRequiredService<IGeneralLedgerRepository>();
        var _bankTransactionRepository = scope.ServiceProvider.GetRequiredService<IBankTransactionRepository>();
        var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        int financialYearEnding = 2025;

        // Need to add bank source, general ledger accounts and general ledger items to the mocked service to ensure 
        // that the service returns a valid summary for the specified financial year ending.
        var addBankSourceRequest = new AddBankSourceRequest("Test Account", "1234567890", "Test Bank", BankType.NAB);
        var bankSourceId = await _bankSourceService.AddBankSourceAsync(addBankSourceRequest);

        var generalLedgerAccounts = new List<GeneralLedgerAccount>
        {
            new GeneralLedgerAccount { Id = 1100, Name = "Assets", AccountType = GeneralLedgerAccountType.Asset },
            new GeneralLedgerAccount { Id = 4000, Name = "Revenue", AccountType = GeneralLedgerAccountType.Revenue },
            new GeneralLedgerAccount { Id = 5000, Name = "Expenses", AccountType = GeneralLedgerAccountType.Expense },
            new GeneralLedgerAccount { Id = 2000, Name = "Liabilities", AccountType = GeneralLedgerAccountType.Liability }
        };

        foreach (var account in generalLedgerAccounts)
        {
            await _generalLedgerService.AddGeneralLedgerAccountAsync(account);
        }

        // Need to add Bank Transactions and corresponding GeneralLedgerItems for the specified financial year ending to ensure that the service returns a valid summary.
        var bankTransactions = new List<BankTransaction>
        {            
            new BankTransaction
            {
                Id = Guid.NewGuid(),
                BankSourceId = bankSourceId,
                TransactionDate = new DateTime(financialYearEnding - 1, 12, 31), // Transaction date is in the previous calendar year, but it belongs to the financial year ending in the specified year
                Amount = -1000m,
                Description = "Test Transaction 1",
                Status = BankTransactionStatus.Reconciled,
                GeneralLedgerItems = new List<GeneralLedgerItem>
                {
                    new GeneralLedgerItem
                    {
                        Id = Guid.NewGuid(),
                        GeneralLedgerAccountId = 1100, // Asset account
                        Amount = 1000m,
                        TransactionDate = new DateTime(financialYearEnding - 1, 12, 31), // Transaction date is in the previous calendar year, but it belongs to the financial year ending in the specified year
                        Side = TransactionSide.Debit
                    },
                    new GeneralLedgerItem
                    {
                        Id = Guid.NewGuid(),
                        GeneralLedgerAccountId = 1000, // Bank transaction for the account
                        Amount = 1000m,
                        TransactionDate = new DateTime(financialYearEnding - 1, 12, 31), // Transaction date is in the previous calendar year, but it belongs to the financial year ending in the specified year
                        Side = TransactionSide.Credit
                    }
                }
            },
            new BankTransaction
            {
                Id = Guid.NewGuid(),
                BankSourceId = bankSourceId,
                TransactionDate = new DateTime(financialYearEnding, 1, 15), // Transaction date is in the specified financial year
                Amount = -500m,
                Description = "Test Transaction 2",
                Status = BankTransactionStatus.Reconciled,
                GeneralLedgerItems = new List<GeneralLedgerItem>
                {
                    new GeneralLedgerItem
                    {
                        Id = Guid.NewGuid(),
                        GeneralLedgerAccountId = 5000, // Expense account
                        Amount = 500m,
                        TransactionDate = new DateTime(financialYearEnding, 1, 15), // Transaction date is in the specified financial year
                        Side = TransactionSide.Debit
                    },
                    new GeneralLedgerItem
                    {
                        Id = Guid.NewGuid(),
                        GeneralLedgerAccountId = 1000, // Bank transaction for the account
                        Amount = 500m,
                        TransactionDate = new DateTime(financialYearEnding, 1, 15), // Transaction date is in the specified financial year
                        Side = TransactionSide.Debit
                    }
                }
            },
            new BankTransaction
            {
                Id = Guid.NewGuid(),
                BankSourceId = bankSourceId,
                TransactionDate = new DateTime(financialYearEnding, 1, 15), // Transaction date is in the specified financial year
                Amount = -800m,
                Description = "Test Transaction 2",
                Status = BankTransactionStatus.Reconciled,
                GeneralLedgerItems = new List<GeneralLedgerItem>
                {
                    new GeneralLedgerItem
                    {
                        Id = Guid.NewGuid(),
                        GeneralLedgerAccountId = 5000, // Expense account
                        Amount = 800m,
                        TransactionDate = new DateTime(financialYearEnding, 1, 15), // Transaction date is in the specified financial year
                        Side = TransactionSide.Debit
                    },
                    new GeneralLedgerItem
                    {
                        Id = Guid.NewGuid(),
                        GeneralLedgerAccountId = 1000, // Bank transaction for the account
                        Amount = 800m,
                        TransactionDate = new DateTime(financialYearEnding, 1, 15), // Transaction date is in the specified financial year
                        Side = TransactionSide.Debit
                    }
                }
            }
        };

        await _bankTransactionRepository.AddTransactionsAsync(bankTransactions);
        
        var expectedSummary = new List<AccountSummaryRowDto>
        {
            new AccountSummaryRowDto
            {
                AccountId = 1100,
                AccountName = "Assets",
                AccountType = GeneralLedgerAccountType.Asset,
                TotalDebits = 1000m,
                TotalCredits = 0m,
                Balance = 1000m
            },
            new AccountSummaryRowDto
            {
                AccountId = 5000,
                AccountName = "Expenses",
                AccountType = GeneralLedgerAccountType.Expense,
                TotalDebits = 1300m, // Total debits from both transactions
                TotalCredits = 0m,
                Balance = 1300m
            },
            new AccountSummaryRowDto
            {
                AccountId = 2000,
                AccountName = "Liability",
                AccountType = GeneralLedgerAccountType.Liability,
                TotalDebits = 0m,
                TotalCredits = 0m,
                Balance = 0m
            },
            new AccountSummaryRowDto
            {
                AccountId = 4000,
                AccountName = "Revenue",
                AccountType = GeneralLedgerAccountType.Revenue,
                TotalDebits = 0m,
                TotalCredits = 0m,
                Balance = 0m
            },                        
        };
        await _unitOfWork.CommitAsync();    

        // Act
        var response = await client.GetAsync($"/api/v1/reports/accounts-summary?financialYearEnding={financialYearEnding}");

        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"API Response: {content}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() } // This is the key
        };

        var actualSummary = await response.Content.ReadFromJsonAsync<List<AccountSummaryRowDto>>(options);
        Assert.NotNull(actualSummary);
        Assert.Equal(expectedSummary.Count, actualSummary.Count);
        for (int i = 0; i < expectedSummary.Count; i++)
        {
            //Assert.Equal(expectedSummary[i].AccountId, actualSummary[i].AccountId);
            Assert.Equal(expectedSummary[i].AccountName, actualSummary[i].AccountName);
            Assert.Equal(expectedSummary[i].AccountType, actualSummary[i].AccountType);
            Assert.Equal(expectedSummary[i].TotalDebits, actualSummary[i].TotalDebits);
            Assert.Equal(expectedSummary[i].TotalCredits, actualSummary[i].TotalCredits);
            Assert.Equal(expectedSummary[i].Balance, actualSummary[i].Balance);
        }
    }
}

using LedgerPro.Core.Enums;

namespace LedgerPro.Application.DTOs.Reports;

public record GlAccountFinancialTotal(
    int AccountId,
    string AccountName,
    GeneralLedgerAccountType AccountType,
    decimal TotalDebits,
    decimal TotalCredits
);

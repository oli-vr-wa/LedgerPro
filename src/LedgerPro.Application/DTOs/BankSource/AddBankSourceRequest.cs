
using LedgerPro.Core.Enums;

namespace LedgerPro.Application.DTOs.BankSource;

public record AddBankSourceRequest(
    string AccountName,
    string AccountNumber,
    string BankName,
    BankType BankType
);

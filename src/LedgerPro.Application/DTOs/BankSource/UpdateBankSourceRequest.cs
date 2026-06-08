
using LedgerPro.Core.Enums;

namespace LedgerPro.Application.DTOs.BankSource;

public record UpdateBankSourceRequest(
    string AccountName,
    string AccountNumber,
    string BankName,
    BankType BankType
);
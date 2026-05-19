
namespace LedgerPro.Application.DTOs.BankStatement;

public record ReconcileBankTransactionRequest(
    Guid BankTransactionId,
    List<SplitGeneralLedgerItemRequest> SplitGeneralLedgerItems
);

public record SplitGeneralLedgerItemRequest(
    int GeneralLedgerAccountId,
    string Description,
    string Reference,
    decimal Amount
);

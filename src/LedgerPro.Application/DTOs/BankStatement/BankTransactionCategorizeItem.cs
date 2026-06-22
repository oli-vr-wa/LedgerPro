
namespace LedgerPro.Application.DTOs.BankStatement;

/// <summary>
/// DTO for categorizing a bank transaction by assigning it to a general ledger account.
/// </summary>
/// <param name="TransactionId">The unique identifier of the bank transaction.</param>
/// <param name="GeneralLedgerAccountId">The unique identifier of the general ledger account.</param>
public class BankTransactionCategorizeItem
{
    public int GeneralLedgerAccountId { get; set; }
    public required string Description { get; set; }
    public string? Reference { get; set; }
    public decimal Amount { get; set; }
}

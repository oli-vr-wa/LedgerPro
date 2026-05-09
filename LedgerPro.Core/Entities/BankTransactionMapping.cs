using LedgerPro.Core.Enums;

namespace LedgerPro.Core.Entities
{
    /// <summary>
    /// Represents a mapping rule that defines how to categorize bank transactions into general ledger accounts based on specific search terms and matching strategies.
    /// </summary>
    public class BankTransactionMapping : BaseGuidEntity
    {
        public string SearchTerm { get; set; } = string.Empty; // e.g., a keyword or pattern to match bank transaction descriptions
        public BankTransactionMatchStrategy MatchStrategy { get; set; } // Strategy to use for matching bank transactions
        public int TargetGeneralLedgerAccountId { get; set; } // Foreign key to the target general ledger account
        public GeneralLedgerAccount TargetGeneralLedgerAccount { get; set; } = null!;
        public string DescriptionTemplate { get; set; } = string.Empty; // Template for the description of the created ledger item
        public string ReferenceTemplate { get; set; } = string.Empty; // Template for the reference of the created ledger item

        public int Priority { get; set; } = 1; // Priority of the mapping (lower number means higher priority)
    }
}
using LedgerPro.Core.Entities;
using LedgerPro.Core.Enums;
using LedgerPro.Core.Interfaces;
using System.Text.RegularExpressions;

namespace LedgerPro.Core.Services
{
    public class TransactionMatchService : ITransactionMatchService
    {
        /// <summary>
        /// Attempts to match a bank transaction against a list of mappings and creates a corresponding GeneralLedgerItem if a match is found.
        /// </summary>
        /// <param name="bankTransaction">The bank transaction to match.</param>
        /// <param name="mappings">The list of mappings to check against.</param>
        /// <returns>The matched GeneralLedgerItem, or null if no match is found.</returns>
        public GeneralLedgerItem? MatchAndCreateLedgerItem(BankTransaction bankTransaction, IEnumerable<BankTransactionMapping> mappings)
        {
            // Sort Mappings by Priority (lower number means higher priority)
            var sortedMappings = mappings.OrderBy(m => m.Priority);

            foreach (var mapping in sortedMappings)
            {
                bool isMatch = false;

                switch (mapping.MatchStrategy)
                {
                    case BankTransactionMatchStrategy.Contains:
                        isMatch = bankTransaction.Description.Contains(mapping.SearchTerm, StringComparison.CurrentCultureIgnoreCase);
                        break;
                    case BankTransactionMatchStrategy.StartsWith:
                        isMatch = bankTransaction.Description.StartsWith(mapping.SearchTerm, StringComparison.CurrentCultureIgnoreCase);
                        break;
                    case BankTransactionMatchStrategy.Exact:
                        isMatch = string.Equals(bankTransaction.Description, mapping.SearchTerm, StringComparison.CurrentCultureIgnoreCase);
                        break;
                    case BankTransactionMatchStrategy.Regex:
                        isMatch = Regex.IsMatch(bankTransaction.Description, mapping.SearchTerm, RegexOptions.IgnoreCase);
                        break;
                }

                if (isMatch)
                {
                    // Update the bank transaction status to Categorized
                    bankTransaction.Status = BankTransactionStatus.Categorized;

                    // Create and return a new GeneralLedgerItem based on the mapping and bank transaction
                    return new GeneralLedgerItem
                    {
                        TransactionDate = bankTransaction.TransactionDate,
                        Reference = mapping.ReferenceTemplate,
                        Description = mapping.DescriptionTemplate,
                        Amount = Math.Abs(bankTransaction.Amount),                        
                        GeneralLedgerAccountId = mapping.TargetGeneralLedgerAccountId,
                        Side = bankTransaction.Amount < 0 ? TransactionSide.Debit : TransactionSide.Credit,                        
                        BankTransaction = bankTransaction,
                    };
                }
            }

            // If no match is found, return null
            return null;
        }
    }
}
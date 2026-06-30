using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LedgerPro.Application.DTOs.GeneralLedgerItem;

/// <summary>
/// Represents a transaction in the general ledger, including details such as description, reference, amount, and the associated general ledger account name.
/// This DTO is used to display GL items that belong to a specific bank transaction.
/// </summary>
public class GeneralLedgerItemTransaction
{
    public string Description { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string GeneralLedgerAccountName { get; set; } = string.Empty;
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LedgerPro.Application.DTOs.BankStatement;

public class BankTransactionCategorize
{
    public Guid BankTransactionId { get; set; }
    public List<BankTransactionCategorizeItem> CategorizeItems { get; set; } = new List<BankTransactionCategorizeItem>();
}

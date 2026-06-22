using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LedgerPro.Application.DTOs.BankStatement;
using LedgerPro.Core.Common;

namespace LedgerPro.Application.Interfaces.Services;

public interface ICategorizationService
{
    Task<Result<bool>> CategorizeBankTransactionAsync(BankTransactionCategorize categorizeDto);
}

using Microsoft.EntityFrameworkCore;
using LedgerPro.Core.Entities;

namespace LedgerPro.Core.Interfaces
{
    public interface ILedgerDbContext
    {
        DbSet<BankSource> BankSources { get; }
        DbSet<BankTransaction> BankTransactions { get; }
        DbSet<BankTransactionMapping> BankTransactionMappings { get; }
        DbSet<FinancialPeriod> FinancialPeriods { get; }
        DbSet<GeneralLedgerAccount> GeneralLedgerAccounts { get; }
        DbSet<GeneralLedgerItem> GeneralLedgerItems { get; }
        DbSet<StatementImport> StatementImports { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
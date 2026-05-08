using Microsoft.EntityFrameworkCore;
using LedgerPro.Core.Entities;

namespace LedgerPro.Core.Interfaces
{
    public interface ILedgerDbContext
    {
        DbSet<BankSource> BankSources { get; }
        DbSet<BankTransaction> BankTransactions { get; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
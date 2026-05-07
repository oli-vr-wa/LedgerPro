using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LedgerPro.Infrastructure
{
    public class LedgerDbContextFactory : IDesignTimeDbContextFactory<LedgerDbContext>
    {
        public LedgerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LedgerDbContext>();
            optionsBuilder.UseSqlite("Data Source=../ledgerpro.db");

            return new LedgerDbContext(optionsBuilder.Options);
        }
    }
}
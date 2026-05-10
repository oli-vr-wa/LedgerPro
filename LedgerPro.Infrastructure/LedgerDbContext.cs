using Microsoft.EntityFrameworkCore;
using LedgerPro.Core.Entities;
using LedgerPro.Core.Interfaces;

namespace LedgerPro.Infrastructure;

public class LedgerDbContext : DbContext
{
    public LedgerDbContext(DbContextOptions<LedgerDbContext> options) : base(options)
    {        
    }

    public DbSet<BankSource> BankSources => Set<BankSource>();
    public DbSet<BankTransaction> BankTransactions => Set<BankTransaction>();
    public DbSet<BankTransactionMapping> BankTransactionMappings => Set<BankTransactionMapping>();
    public DbSet<FinancialPeriod> FinancialPeriods => Set<FinancialPeriod>();
    public DbSet<GeneralLedgerAccount> GeneralLedgerAccounts => Set<GeneralLedgerAccount>();
    public DbSet<GeneralLedgerItem> GeneralLedgerItems => Set<GeneralLedgerItem>();
    public DbSet<StatementImport> StatementImports => Set<StatementImport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BankTransaction>()
            .Property(bt => bt.Amount)
            .HasConversion<double>();
            
        modelBuilder.Entity<GeneralLedgerItem>()
            .Property(gli => gli.Amount)
            .HasConversion<double>();

        // Configure all Guid properties to be stored as strings in the database
        // This is necessary because SQLite does not have a native Guid type, and storing Guids as strings ensures compatibility 
        // and prevents issues with data retrieval and querying.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(Guid) || property.ClrType == typeof(Guid?))
                {
                    property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.GuidToStringConverter());
                }
            }
        }
    }
}

using LedgerPro.Core.Interfaces;

namespace LedgerPro.Infrastructure.Repositories;

/// <summary>
/// Implements the Unit of Work pattern to manage database transactions. 
/// This class provides a single point of entry for committing changes to the database, 
/// ensuring that all operations performed through the repositories are part of a single transaction. 
/// </summary>
/// <param name="dbContext"></param>
public class UnitOfWork(LedgerDbContext dbContext) : IUnitOfWork
{
    private readonly LedgerDbContext _dbContext = dbContext;

    /// <summary>
    /// Commits all changes made in the context to the database. 
    /// This method should be called after performing operations through the repositories to persist the changes.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

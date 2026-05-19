namespace LedgerPro.Application.Interfaces.Repositories;

public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
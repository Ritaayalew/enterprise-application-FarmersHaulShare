namespace IdentityAndAccessManagement.Application.Abstractions;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task<IDisposable> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(IDisposable transaction);
    Task RollbackTransactionAsync(IDisposable transaction);
}
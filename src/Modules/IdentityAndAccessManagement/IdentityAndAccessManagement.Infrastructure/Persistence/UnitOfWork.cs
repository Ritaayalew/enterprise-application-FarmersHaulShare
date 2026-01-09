using IdentityAndAccessManagement.Application.Abstractions;
using IdentityAndAccessManagement.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace IdentityAndAccessManagement.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly IdentityDbContext _db;

    public UnitOfWork(IdentityDbContext db) => _db = db;

    public async Task CommitAsync(CancellationToken ct = default)
    {
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IDisposable> BeginTransactionAsync(CancellationToken ct = default)
    {
        var tx = await _db.Database.BeginTransactionAsync(ct);
        return tx;
    }

    public async Task CommitTransactionAsync(IDisposable transaction)
    {
        if (transaction is IDbContextTransaction dbTx)
        {
            await dbTx.CommitAsync();
            await dbTx.DisposeAsync();
        }
    }

    public async Task RollbackTransactionAsync(IDisposable transaction)
    {
        if (transaction is IDbContextTransaction dbTx)
        {
            await dbTx.RollbackAsync();
            await dbTx.DisposeAsync();
        }
    }
}

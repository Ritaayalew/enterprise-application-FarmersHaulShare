using BatchPostingAndGrouping.Domain.Entities;
using BatchPostingAndGrouping.Domain.Repositories;
using BatchPostingAndGrouping.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BatchPostingAndGrouping.Infrastructure.Repositories;

public sealed class BatchRepository : IBatchRepository
{
    private readonly BatchPostingDbContext _context;

    public BatchRepository(BatchPostingDbContext context)
    {
        _context = context;
    }

    public async Task<Batch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Batches
            .Include(b => b.FarmerProfile)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Batch>> GetByFarmerProfileIdAsync(
        Guid farmerProfileId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Batches
            .Where(b => b.FarmerProfileId == farmerProfileId)
            .OrderByDescending(b => b.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Batch>> GetAvailableBatchesAsync(
        DateTime? readyFrom = null,
        DateTime? readyTo = null,
        string? produceTypeName = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Batches
            .Where(b => b.Status == BatchStatus.Available)
            .AsQueryable();

        if (readyFrom.HasValue)
            query = query.Where(b => b.ReadyDateUtc >= readyFrom.Value);

        if (readyTo.HasValue)
            query = query.Where(b => b.ReadyDateUtc <= readyTo.Value);

        if (!string.IsNullOrWhiteSpace(produceTypeName))
            query = query.Where(b => b.ProduceType.Name == produceTypeName);

        return await query
            .OrderBy(b => b.ReadyDateUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Batch batch, CancellationToken cancellationToken = default)
    {
        await _context.Batches.AddAsync(batch, cancellationToken);
    }

    public async Task UpdateAsync(Batch batch, CancellationToken cancellationToken = default)
    {
        _context.Batches.Update(batch);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Batch batch, CancellationToken cancellationToken = default)
    {
        _context.Batches.Remove(batch);
        await Task.CompletedTask;
    }
}

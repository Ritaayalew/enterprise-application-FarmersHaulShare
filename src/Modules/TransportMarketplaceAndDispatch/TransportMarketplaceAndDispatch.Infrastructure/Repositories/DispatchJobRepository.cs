using Microsoft.EntityFrameworkCore;
using TransportMarketplaceAndDispatch.Domain.Aggregates;
using TransportMarketplaceAndDispatch.Domain.Repositories;
using TransportMarketplaceAndDispatch.Infrastructure.Persistence;

namespace TransportMarketplaceAndDispatch.Infrastructure.Repositories;

public sealed class DispatchJobRepository : IDispatchJobRepository
{
    private readonly TransportDbContext _dbContext;

    public DispatchJobRepository(TransportDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DispatchJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DispatchJobs
            .FirstOrDefaultAsync(dj => dj.Id == id, cancellationToken);
    }

    public async Task<DispatchJob?> GetByHaulShareIdAsync(Guid haulShareId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DispatchJobs
            .FirstOrDefaultAsync(dj => dj.HaulShareId == haulShareId, cancellationToken);
    }

    public async Task<IReadOnlyList<DispatchJob>> GetByDriverIdAsync(Guid driverId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DispatchJobs
            .Where(dj => dj.AssignedDriverId == driverId)
            .OrderByDescending(dj => dj.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DispatchJob>> GetAvailableJobsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.DispatchJobs
            .Where(dj => dj.Status == DispatchJobStatus.Posted)
            .OrderBy(dj => dj.ScheduledPickupTime)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(DispatchJob dispatchJob, CancellationToken cancellationToken = default)
    {
        await _dbContext.DispatchJobs.AddAsync(dispatchJob, cancellationToken);
    }

    public async Task UpdateAsync(DispatchJob dispatchJob, CancellationToken cancellationToken = default)
    {
        _dbContext.DispatchJobs.Update(dispatchJob);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
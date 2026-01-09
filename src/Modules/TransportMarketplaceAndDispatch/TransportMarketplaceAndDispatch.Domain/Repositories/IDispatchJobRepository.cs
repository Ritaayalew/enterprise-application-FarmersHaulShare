using TransportMarketplaceAndDispatch.Domain.Aggregates;

namespace TransportMarketplaceAndDispatch.Domain.Repositories;

public interface IDispatchJobRepository
{
    Task<DispatchJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DispatchJob?> GetByHaulShareIdAsync(Guid haulShareId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DispatchJob>> GetByDriverIdAsync(Guid driverId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DispatchJob>> GetAvailableJobsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(DispatchJob dispatchJob, CancellationToken cancellationToken = default);
    Task UpdateAsync(DispatchJob dispatchJob, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
using TransportMarketplaceAndDispatch.Domain.Aggregates;

namespace TransportMarketplaceAndDispatch.Domain.Repositories;

public interface IDriverRepository
{
    Task<Driver?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Driver?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Driver>> GetAvailableDriversAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Driver driver, CancellationToken cancellationToken = default);
    Task UpdateAsync(Driver driver, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
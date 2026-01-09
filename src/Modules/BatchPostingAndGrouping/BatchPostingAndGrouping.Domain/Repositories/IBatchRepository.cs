using BatchPostingAndGrouping.Domain.Entities;

namespace BatchPostingAndGrouping.Domain.Repositories;

public interface IBatchRepository
{
    Task<Batch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Batch>> GetByFarmerProfileIdAsync(Guid farmerProfileId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Batch>> GetAvailableBatchesAsync(
        DateTime? readyFrom = null,
        DateTime? readyTo = null,
        string? produceTypeName = null,
        CancellationToken cancellationToken = default);
    Task AddAsync(Batch batch, CancellationToken cancellationToken = default);
    Task UpdateAsync(Batch batch, CancellationToken cancellationToken = default);
    Task DeleteAsync(Batch batch, CancellationToken cancellationToken = default);
}

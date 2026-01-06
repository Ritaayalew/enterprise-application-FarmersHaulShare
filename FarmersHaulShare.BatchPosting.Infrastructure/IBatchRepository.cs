using FarmersHaulShare.BatchPosting.Domain.Aggregates;

namespace FarmersHaulShare.BatchPosting.Infrastructure;

public interface IBatchRepository
{
    Task AddAsync(Batch batch, CancellationToken cancellationToken = default);
}
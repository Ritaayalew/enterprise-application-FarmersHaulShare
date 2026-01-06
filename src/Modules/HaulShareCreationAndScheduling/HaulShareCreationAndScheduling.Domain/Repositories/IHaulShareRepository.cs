using HaulShareCreationAndScheduling.Domain.Aggregates;

namespace HaulShareCreationAndScheduling.Domain.Repositories;

public interface IHaulShareRepository
{
    Task AddAsync(HaulShare haulShare, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

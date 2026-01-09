using HaulShareCreationAndScheduling.Domain.Aggregates;
using HaulShareCreationAndScheduling.Domain.Repositories;
using HaulShareCreationAndScheduling.Infrastructure.Persistence;

namespace HaulShareCreationAndScheduling.Infrastructure.Repositories;

public class HaulShareRepository : IHaulShareRepository
{
    private readonly HaulShareDbContext _dbContext;

    public HaulShareRepository(HaulShareDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(HaulShare haulShare, CancellationToken cancellationToken)
    {
        await _dbContext.HaulShares.AddAsync(haulShare, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

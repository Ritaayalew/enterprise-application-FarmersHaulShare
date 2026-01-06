using FarmersHaulShare.BatchPosting.Domain.Aggregates;
using Microsoft.EntityFrameworkCore;


namespace FarmersHaulShare.BatchPosting.Infrastructure;

public class BatchRepository : IBatchRepository
{
    private readonly BatchPostingDbContext _context;

    public BatchRepository(BatchPostingDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Batch batch, CancellationToken cancellationToken = default)
    {
        await _context.Batches.AddAsync(batch, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken); // This triggers Outbox save
    }
}
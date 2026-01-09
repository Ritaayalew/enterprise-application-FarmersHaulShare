using BatchPostingAndGrouping.Domain.Aggregates;
using BatchPostingAndGrouping.Domain.Repositories;
using BatchPostingAndGrouping.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BatchPostingAndGrouping.Infrastructure.Repositories;

public sealed class GroupCandidateRepository : IGroupCandidateRepository
{
    private readonly BatchPostingDbContext _context;

    public GroupCandidateRepository(BatchPostingDbContext context)
    {
        _context = context;
    }

    public async Task<GroupCandidate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.GroupCandidates
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<GroupCandidate>> GetFormingCandidatesAsync(
        string? produceTypeName = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.GroupCandidates
            .Where(g => g.Status == GroupCandidateStatus.Forming)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(produceTypeName))
            query = query.Where(g => g.ProduceType.Name == produceTypeName);

        return await query
            .OrderBy(g => g.EarliestReadyDateUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GroupCandidate>> GetLockedCandidatesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.GroupCandidates
            .Where(g => g.Status == GroupCandidateStatus.Locked)
            .OrderBy(g => g.LockWindowStartUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(GroupCandidate groupCandidate, CancellationToken cancellationToken = default)
    {
        // EF Core will handle the conversion automatically through the configuration
        await _context.GroupCandidates.AddAsync(groupCandidate, cancellationToken);
    }

    public async Task UpdateAsync(GroupCandidate groupCandidate, CancellationToken cancellationToken = default)
    {
        // EF Core will handle the conversion automatically through the configuration
        _context.GroupCandidates.Update(groupCandidate);
        await Task.CompletedTask;
    }
}

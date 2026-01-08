using BatchPostingAndGrouping.Domain.Aggregates;

namespace BatchPostingAndGrouping.Domain.Repositories;

public interface IGroupCandidateRepository
{
    Task<GroupCandidate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GroupCandidate>> GetFormingCandidatesAsync(
        string? produceTypeName = null,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GroupCandidate>> GetLockedCandidatesAsync(CancellationToken cancellationToken = default);
    Task AddAsync(GroupCandidate groupCandidate, CancellationToken cancellationToken = default);
    Task UpdateAsync(GroupCandidate groupCandidate, CancellationToken cancellationToken = default);
}

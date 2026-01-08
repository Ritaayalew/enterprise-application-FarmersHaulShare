using BatchPostingAndGrouping.Application.DTOs;
using BatchPostingAndGrouping.Domain.DomainEvents;

namespace BatchPostingAndGrouping.Application.Services;

public interface IGroupingService
{
    Task<IReadOnlyList<GroupCandidateDto>> GetFormingGroupCandidatesAsync(
        string? produceTypeName = null,
        CancellationToken cancellationToken = default);
    Task<GroupCandidateDto?> GetGroupCandidateByIdAsync(Guid groupCandidateId, CancellationToken cancellationToken = default);
    Task<GroupCandidateDto> FormGroupCandidateAsync(
        List<Guid> batchIds,
        double maxDistanceKm = 50.0,
        TimeSpan? maxTimeWindowHours = null,
        CancellationToken cancellationToken = default);
    Task<GroupCandidateLocked> LockGroupCandidateAsync(
        Guid groupCandidateId,
        DateTime lockWindowStartUtc,
        DateTime lockWindowEndUtc,
        CancellationToken cancellationToken = default);
    Task ProcessBatchPostedEventAsync(BatchPosted batchPosted, CancellationToken cancellationToken = default);
}

using FarmersHaulShare.SharedKernel.Domain;

namespace BatchPostingAndGrouping.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a group candidate is locked (ready for haul share creation)
/// </summary>
public sealed record GroupCandidateLocked : IDomainEvent
{
    public Guid GroupCandidateId { get; init; }
    public List<Guid> BatchIds { get; init; } = new();
    public DateTime LockWindowStartUtc { get; init; }
    public DateTime LockWindowEndUtc { get; init; }
    public DateTime OccurredOn { get; init; }

    private GroupCandidateLocked() { } // For deserialization

    public GroupCandidateLocked(
        Guid groupCandidateId,
        List<Guid> batchIds,
        DateTime lockWindowStartUtc,
        DateTime lockWindowEndUtc)
    {
        if (batchIds == null || batchIds.Count == 0)
            throw new SharedKernel.Domain.DomainException("Group candidate must contain at least one batch.");

        GroupCandidateId = groupCandidateId;
        BatchIds = batchIds;
        LockWindowStartUtc = lockWindowStartUtc;
        LockWindowEndUtc = lockWindowEndUtc;
        OccurredOn = DateTime.UtcNow;
    }
}

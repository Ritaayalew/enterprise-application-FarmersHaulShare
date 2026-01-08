using SharedKernel.Domain;
using BatchPostingAndGrouping.Domain.DomainEvents;
using BatchPostingAndGrouping.Domain.Entities;
using BatchPostingAndGrouping.Domain.ValueObjects;

using System.Diagnostics.CodeAnalysis;

namespace BatchPostingAndGrouping.Domain.Aggregates;

/// <summary>
/// Aggregate root representing a candidate group of batches that can be combined into a haul share
/// </summary>
public sealed class GroupCandidate : AggregateRoot<Guid>
{
    private readonly List<Guid> _batchIds = new();
    public IReadOnlyCollection<Guid> BatchIds => _batchIds.AsReadOnly();

    public ProduceType ProduceType { get; private init; } = null!;
    public double TotalWeightInKg { get; private set; }
    
    public DateTime EarliestReadyDateUtc { get; private set; }
    public DateTime LatestReadyDateUtc { get; private set; }
    public DateTime? PickupWindowEndUtc { get; private set; }

    public GroupCandidateStatus Status { get; private set; }
    
    // Lock window for accepting this group
    public DateTime? LockWindowStartUtc { get; private set; }
    public DateTime? LockWindowEndUtc { get; private set; }
    
    public DateTime CreatedAtUtc { get; private init; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime? LockedAtUtc { get; private set; }

    private GroupCandidate() { } // EF Core

    [SetsRequiredMembers]
    public GroupCandidate(
        Guid id,
        ProduceType produceType,
        DateTime earliestReadyDateUtc,
        DateTime latestReadyDateUtc,
        DateTime? pickupWindowEndUtc = null)
        : base(id)
    {
        if (earliestReadyDateUtc > latestReadyDateUtc)
            throw new SharedKernel.Domain.DomainException("Earliest ready date must be before or equal to latest ready date.");

        Id = id; // Explicitly set for required property (base(id) also sets it)
        ProduceType = produceType ?? throw new ArgumentNullException(nameof(produceType));
        EarliestReadyDateUtc = earliestReadyDateUtc;
        LatestReadyDateUtc = latestReadyDateUtc;
        PickupWindowEndUtc = pickupWindowEndUtc;
        Status = GroupCandidateStatus.Forming;
        CreatedAtUtc = DateTime.UtcNow;
        TotalWeightInKg = 0;
    }

    /// <summary>
    /// Adds a batch to this group candidate if it meets grouping constraints
    /// </summary>
    public void AddBatch(Batch batch, double maxDistanceKm = 50.0, TimeSpan maxTimeWindowHours = default)
    {
        if (Status != GroupCandidateStatus.Forming)
            throw new SharedKernel.Domain.DomainException($"Cannot add batch to group candidate with status {Status}.");

        if (batch.Status != BatchStatus.Available)
            throw new SharedKernel.Domain.DomainException("Cannot add a batch that is not available.");

        if (_batchIds.Contains(batch.Id))
            throw new SharedKernel.Domain.DomainException("Batch is already in this group candidate.");

        // Validate produce type matches
        if (!ProduceType.Name.Equals(batch.ProduceType.Name, StringComparison.OrdinalIgnoreCase))
            throw new SharedKernel.Domain.DomainException(
                $"Batch produce type '{batch.ProduceType.Name}' does not match group produce type '{ProduceType.Name}'.");

        // Validate time window (if other batches exist)
        if (_batchIds.Count > 0)
        {
            if (maxTimeWindowHours == default)
                maxTimeWindowHours = TimeSpan.FromHours(24); // Default 24 hours window

            var timeWindow = LatestReadyDateUtc - EarliestReadyDateUtc;
            var newTimeWindow = batch.ReadyDateUtc < EarliestReadyDateUtc
                ? LatestReadyDateUtc - batch.ReadyDateUtc
                : batch.ReadyDateUtc - EarliestReadyDateUtc;

            if (timeWindow + newTimeWindow > maxTimeWindowHours)
                throw new SharedKernel.Domain.DomainException(
                    $"Adding this batch would exceed the maximum time window of {maxTimeWindowHours.TotalHours} hours.");
        }

        _batchIds.Add(batch.Id);
        TotalWeightInKg += batch.WeightInKg;

        // Update time windows
        if (batch.ReadyDateUtc < EarliestReadyDateUtc)
            EarliestReadyDateUtc = batch.ReadyDateUtc;
        if (batch.ReadyDateUtc > LatestReadyDateUtc)
            LatestReadyDateUtc = batch.ReadyDateUtc;

        if (batch.PickupWindowEndUtc.HasValue)
        {
            if (!PickupWindowEndUtc.HasValue || batch.PickupWindowEndUtc.Value > PickupWindowEndUtc.Value)
                PickupWindowEndUtc = batch.PickupWindowEndUtc.Value;
        }

        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes a batch from the group candidate
    /// </summary>
    public void RemoveBatch(Guid batchId)
    {
        if (Status != GroupCandidateStatus.Forming)
            throw new SharedKernel.Domain.DomainException($"Cannot remove batch from group candidate with status {Status}.");

        if (!_batchIds.Remove(batchId))
            throw new SharedKernel.Domain.DomainException("Batch is not in this group candidate.");

        UpdatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Locks the group candidate, making it ready for haul share creation
    /// Raises GroupCandidateLocked domain event
    /// </summary>
    public GroupCandidateLocked Lock(DateTime lockWindowStartUtc, DateTime lockWindowEndUtc)
    {
        if (Status != GroupCandidateStatus.Forming)
            throw new SharedKernel.Domain.DomainException($"Cannot lock group candidate with status {Status}.");

        if (_batchIds.Count == 0)
            throw new SharedKernel.Domain.DomainException("Cannot lock an empty group candidate.");

        if (lockWindowEndUtc <= lockWindowStartUtc)
            throw new SharedKernel.Domain.DomainException("Lock window end must be after lock window start.");

        Status = GroupCandidateStatus.Locked;
        LockWindowStartUtc = lockWindowStartUtc;
        LockWindowEndUtc = lockWindowEndUtc;
        LockedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;

        var @event = new GroupCandidateLocked(
            Id,
            _batchIds.ToList(),
            lockWindowStartUtc,
            lockWindowEndUtc);

        RaiseDomainEvent(@event);
        return @event;
    }

    /// <summary>
    /// Checks if the lock window is still valid
    /// </summary>
    public bool IsLockWindowValid(DateTime nowUtc)
    {
        if (Status != GroupCandidateStatus.Locked)
            return false;

        if (!LockWindowStartUtc.HasValue || !LockWindowEndUtc.HasValue)
            return false;

        return nowUtc >= LockWindowStartUtc.Value && nowUtc <= LockWindowEndUtc.Value;
    }

    /// <summary>
    /// Expires the group candidate if lock window has passed
    /// </summary>
    public void ExpireIfLockWindowPassed(DateTime nowUtc)
    {
        if (Status == GroupCandidateStatus.Locked && 
            LockWindowEndUtc.HasValue && 
            nowUtc > LockWindowEndUtc.Value)
        {
            Status = GroupCandidateStatus.Expired;
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }

    public int BatchCount => _batchIds.Count;
}

public enum GroupCandidateStatus
{
    Forming = 0,    // Still accepting batches
    Locked = 1,     // Locked and ready for haul share creation
    Expired = 2,    // Lock window passed, no longer valid
    Converted = 3   // Successfully converted to haul share
}

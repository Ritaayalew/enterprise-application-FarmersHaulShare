using BatchPostingAndGrouping.Application.DTOs;
using BatchPostingAndGrouping.Domain.Aggregates;
using BatchPostingAndGrouping.Domain.DomainEvents;
using BatchPostingAndGrouping.Domain.Entities;
using BatchPostingAndGrouping.Domain.Repositories;
using BatchPostingAndGrouping.Domain.ValueObjects;
using SharedKernel.Domain;

namespace BatchPostingAndGrouping.Application.Services;

public sealed class GroupingService : IGroupingService
{
    private readonly IGroupCandidateRepository _groupCandidateRepository;
    private readonly IBatchRepository _batchRepository;
    private const double DefaultMaxDistanceKm = 50.0;
    private static readonly TimeSpan DefaultMaxTimeWindowHours = TimeSpan.FromHours(24);

    public GroupingService(
        IGroupCandidateRepository groupCandidateRepository,
        IBatchRepository batchRepository)
    {
        _groupCandidateRepository = groupCandidateRepository;
        _batchRepository = batchRepository;
    }

    public async Task<IReadOnlyList<GroupCandidateDto>> GetFormingGroupCandidatesAsync(
        string? produceTypeName = null,
        CancellationToken cancellationToken = default)
    {
        var candidates = await _groupCandidateRepository.GetFormingCandidatesAsync(produceTypeName, cancellationToken);
        return candidates.Select(MapToDto).ToList();
    }

    public async Task<GroupCandidateDto?> GetGroupCandidateByIdAsync(
        Guid groupCandidateId,
        CancellationToken cancellationToken = default)
    {
        var candidate = await _groupCandidateRepository.GetByIdAsync(groupCandidateId, cancellationToken);
        return candidate == null ? null : MapToDto(candidate);
    }

    public async Task<GroupCandidateDto> FormGroupCandidateAsync(
        List<Guid> batchIds,
        double maxDistanceKm = DefaultMaxDistanceKm,
        TimeSpan? maxTimeWindowHours = null,
        CancellationToken cancellationToken = default)
    {
        if (batchIds == null || batchIds.Count == 0)
            throw new DomainException("At least one batch ID is required to form a group candidate.");

        if (batchIds.Count == 1)
            throw new DomainException("At least two batches are required to form a group candidate.");

        // Load all batches
        var batches = new List<Batch>();
        foreach (var batchId in batchIds)
        {
            var batch = await _batchRepository.GetByIdAsync(batchId, cancellationToken);
            if (batch == null)
                throw new DomainException($"Batch with ID {batchId} not found.");
            if (batch.Status != BatchStatus.Available)
                throw new DomainException($"Batch with ID {batchId} is not available for grouping.");
            batches.Add(batch);
        }

        // Validate all batches have the same produce type
        var firstBatch = batches[0];
        if (batches.Any(b => !b.ProduceType.Name.Equals(firstBatch.ProduceType.Name, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException("All batches must have the same produce type.");

        // Calculate time windows
        var earliestReadyDate = batches.Min(b => b.ReadyDateUtc);
        var latestReadyDate = batches.Max(b => b.ReadyDateUtc);
        var pickupWindowEnd = batches
            .Where(b => b.PickupWindowEndUtc.HasValue)
            .Select(b => b.PickupWindowEndUtc!.Value)
            .DefaultIfEmpty()
            .Max();

        // Create group candidate
        var groupCandidateId = Guid.NewGuid();
        var produceType = new ProduceType(
            firstBatch.ProduceType.Name,
            firstBatch.ProduceType.Category,
            firstBatch.ProduceType.Unit);

        var groupCandidate = new GroupCandidate(
            groupCandidateId,
            produceType,
            earliestReadyDate,
            latestReadyDate,
            pickupWindowEnd == default(DateTime) ? null : pickupWindowEnd);

        // Add batches to group candidate
        var timeWindow = maxTimeWindowHours ?? DefaultMaxTimeWindowHours;
        foreach (var batch in batches.Skip(1)) // First batch is used for initial setup
        {
            groupCandidate.AddBatch(batch, maxDistanceKm, timeWindow);
        }
        // Add first batch explicitly
        groupCandidate.AddBatch(firstBatch, maxDistanceKm, timeWindow);

        await _groupCandidateRepository.AddAsync(groupCandidate, cancellationToken);

        return MapToDto(groupCandidate);
    }

    public async Task<GroupCandidateLocked> LockGroupCandidateAsync(
        Guid groupCandidateId,
        DateTime lockWindowStartUtc,
        DateTime lockWindowEndUtc,
        CancellationToken cancellationToken = default)
    {
        var groupCandidate = await _groupCandidateRepository.GetByIdAsync(groupCandidateId, cancellationToken);
        if (groupCandidate == null)
            throw new DomainException($"Group candidate with ID {groupCandidateId} not found.");

        var @event = groupCandidate.Lock(lockWindowStartUtc, lockWindowEndUtc);

        // Mark all batches as grouped
        foreach (var batchId in groupCandidate.BatchIds)
        {
            var batch = await _batchRepository.GetByIdAsync(batchId, cancellationToken);
            if (batch != null)
            {
                batch.MarkAsGrouped();
                await _batchRepository.UpdateAsync(batch, cancellationToken);
            }
        }

        await _groupCandidateRepository.UpdateAsync(groupCandidate, cancellationToken);
        return @event;
    }

    public async Task ProcessBatchPostedEventAsync(
        BatchPosted batchPosted,
        CancellationToken cancellationToken = default)
    {
        // Load the batch
        var batch = await _batchRepository.GetByIdAsync(batchPosted.BatchId, cancellationToken);
        if (batch == null || batch.Status != BatchStatus.Available)
            return;

        // Try to find an existing forming group candidate with the same produce type
        var existingCandidates = await _groupCandidateRepository.GetFormingCandidatesAsync(
            batchPosted.ProduceTypeName,
            cancellationToken);

        GroupCandidate? matchedCandidate = null;
        foreach (var candidate in existingCandidates)
        {
            try
            {
                // Try to add batch to this candidate
                candidate.AddBatch(batch, DefaultMaxDistanceKm, DefaultMaxTimeWindowHours);
                matchedCandidate = candidate;
                break;
            }
            catch (DomainException)
            {
                // This candidate doesn't match constraints, try next one
                continue;
            }
        }

        if (matchedCandidate != null)
        {
            await _groupCandidateRepository.UpdateAsync(matchedCandidate, cancellationToken);
        }
        else
        {
            // Create a new group candidate with just this batch
            // Note: We still create it even with one batch to allow future batches to join
            var produceType = batch.ProduceType;
            var groupCandidate = new GroupCandidate(
                Guid.NewGuid(),
                produceType,
                batch.ReadyDateUtc,
                batch.ReadyDateUtc,
                batch.PickupWindowEndUtc);

            groupCandidate.AddBatch(batch, DefaultMaxDistanceKm, DefaultMaxTimeWindowHours);
            await _groupCandidateRepository.AddAsync(groupCandidate, cancellationToken);
        }
    }

    private static GroupCandidateDto MapToDto(GroupCandidate candidate)
    {
        return new GroupCandidateDto
        {
            Id = candidate.Id,
            BatchIds = candidate.BatchIds.ToList(),
            ProduceTypeName = candidate.ProduceType.Name,
            TotalWeightInKg = candidate.TotalWeightInKg,
            EarliestReadyDateUtc = candidate.EarliestReadyDateUtc,
            LatestReadyDateUtc = candidate.LatestReadyDateUtc,
            PickupWindowEndUtc = candidate.PickupWindowEndUtc,
            Status = candidate.Status.ToString(),
            LockWindowStartUtc = candidate.LockWindowStartUtc,
            LockWindowEndUtc = candidate.LockWindowEndUtc,
            CreatedAtUtc = candidate.CreatedAtUtc,
            BatchCount = candidate.BatchCount
        };
    }
}

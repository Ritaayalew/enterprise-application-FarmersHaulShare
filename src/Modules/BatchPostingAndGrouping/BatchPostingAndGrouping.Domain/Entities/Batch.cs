using SharedKernel.Domain;
using BatchPostingAndGrouping.Domain.DomainEvents;
using BatchPostingAndGrouping.Domain.ValueObjects;

using System.Diagnostics.CodeAnalysis;

namespace BatchPostingAndGrouping.Domain.Entities;

/// <summary>
/// Entity representing a batch of produce that a farmer is ready to sell/deliver
/// </summary>
public sealed class Batch : Entity<Guid>
{
    public Guid FarmerProfileId { get; private init; }
    public FarmerProfile FarmerProfile { get; private init; } = null!;

    public ProduceType ProduceType { get; private set; } = null!;
    public QualityGrade QualityGrade { get; private set; } = null!;
    public double WeightInKg { get; private set; }
    public Location PickupLocation { get; private set; } = null!;

    public DateTime ReadyDateUtc { get; private set; }
    public DateTime? PickupWindowEndUtc { get; private set; } // Optional end of pickup window

    public BatchStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private init; }
    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public string? CancellationReason { get; private set; }

    private Batch() { } // EF Core

    [SetsRequiredMembers]
    public Batch(
        Guid id,
        Guid farmerProfileId,
        ProduceType produceType,
        QualityGrade qualityGrade,
        double weightInKg,
        Location pickupLocation,
        DateTime readyDateUtc,
        DateTime? pickupWindowEndUtc = null)
        : base(id)
    {
        if (weightInKg <= 0)
            throw new SharedKernel.Domain.DomainException("Batch weight must be greater than zero.");
        if (readyDateUtc < DateTime.UtcNow.AddHours(-1)) // Allow 1 hour grace for clock skew
            throw new SharedKernel.Domain.DomainException("Ready date cannot be in the past.");
        if (pickupWindowEndUtc.HasValue && pickupWindowEndUtc.Value <= readyDateUtc)
            throw new SharedKernel.Domain.DomainException("Pickup window end must be after ready date.");

        Id = id; // Explicitly set for required property (base(id) also sets it)
        FarmerProfileId = farmerProfileId;
        ProduceType = produceType ?? throw new ArgumentNullException(nameof(produceType));
        QualityGrade = qualityGrade ?? throw new ArgumentNullException(nameof(qualityGrade));
        WeightInKg = weightInKg;
        PickupLocation = pickupLocation ?? throw new ArgumentNullException(nameof(pickupLocation));
        ReadyDateUtc = readyDateUtc;
        PickupWindowEndUtc = pickupWindowEndUtc;
        Status = BatchStatus.Available;
        CreatedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the batch as posted and raises BatchPosted domain event
    /// This should be called after the batch is persisted
    /// </summary>
    public BatchPosted Post()
    {
        if (Status != BatchStatus.Available)
            throw new SharedKernel.Domain.DomainException($"Cannot post batch with status {Status}.");

        return new BatchPosted(
            Id,
            FarmerProfileId,
            ProduceType.Name,
            WeightInKg,
            PickupLocation.Latitude,
            PickupLocation.Longitude,
            ReadyDateUtc);
    }

    public void UpdateProduceType(ProduceType produceType)
    {
        if (Status != BatchStatus.Available)
            throw new SharedKernel.Domain.DomainException($"Cannot update batch with status {Status}.");

        ProduceType = produceType ?? throw new ArgumentNullException(nameof(produceType));
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateQualityGrade(QualityGrade qualityGrade)
    {
        if (Status != BatchStatus.Available)
            throw new SharedKernel.Domain.DomainException($"Cannot update batch with status {Status}.");

        QualityGrade = qualityGrade ?? throw new ArgumentNullException(nameof(qualityGrade));
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateWeight(double weightInKg)
    {
        if (Status != BatchStatus.Available)
            throw new SharedKernel.Domain.DomainException($"Cannot update batch with status {Status}.");
        if (weightInKg <= 0)
            throw new SharedKernel.Domain.DomainException("Batch weight must be greater than zero.");

        WeightInKg = weightInKg;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdatePickupLocation(Location location)
    {
        if (Status != BatchStatus.Available)
            throw new SharedKernel.Domain.DomainException($"Cannot update batch with status {Status}.");

        PickupLocation = location ?? throw new ArgumentNullException(nameof(location));
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateReadyDate(DateTime readyDateUtc, DateTime? pickupWindowEndUtc = null)
    {
        if (Status != BatchStatus.Available)
            throw new SharedKernel.Domain.DomainException($"Cannot update batch with status {Status}.");
        if (readyDateUtc < DateTime.UtcNow.AddHours(-1))
            throw new SharedKernel.Domain.DomainException("Ready date cannot be in the past.");
        if (pickupWindowEndUtc.HasValue && pickupWindowEndUtc.Value <= readyDateUtc)
            throw new SharedKernel.Domain.DomainException("Pickup window end must be after ready date.");

        ReadyDateUtc = readyDateUtc;
        PickupWindowEndUtc = pickupWindowEndUtc;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        if (Status == BatchStatus.Cancelled)
            throw new SharedKernel.Domain.DomainException("Batch is already cancelled.");
        if (Status == BatchStatus.Grouped)
            throw new SharedKernel.Domain.DomainException("Cannot cancel a batch that is already grouped.");

        Status = BatchStatus.Cancelled;
        CancelledAtUtc = DateTime.UtcNow;
        CancellationReason = reason?.Trim();
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void MarkAsGrouped()
    {
        if (Status != BatchStatus.Available)
            throw new SharedKernel.Domain.DomainException($"Cannot group batch with status {Status}.");

        Status = BatchStatus.Grouped;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

public enum BatchStatus
{
    Available = 0,
    Grouped = 1,
    Cancelled = 2
}

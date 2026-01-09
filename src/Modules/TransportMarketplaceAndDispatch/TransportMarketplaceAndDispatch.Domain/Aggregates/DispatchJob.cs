using SharedKernel.Domain;
using FarmersHaulShare.SharedKernel.Domain;
using TransportMarketplaceAndDispatch.Domain.Events;
using TransportMarketplaceAndDispatch.Domain.ValueObjects;
using System.Diagnostics.CodeAnalysis;

namespace TransportMarketplaceAndDispatch.Domain.Aggregates;

public sealed class DispatchJob : AggregateRoot<Guid>
{

    public Guid HaulShareId { get; private set; }
    public Route Route { get; private set; } = null!;
    public DateTime ScheduledPickupTime { get; private set; }
    public DateTime EstimatedDeliveryTime { get; private set; }
    public DispatchJobStatus Status { get; private set; }
    public Guid? AssignedDriverId { get; private set; }
    public Location? CurrentLocation { get; private set; }
    public DateTime? PickupStartedAt { get; private set; }
    public DateTime? PickupCompletedAt { get; private set; }
    public DateTime? DeliveryStartedAt { get; private set; }
    public DateTime? DeliveryCompletedAt { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private DispatchJob() { } // EF Core

    [SetsRequiredMembers]
    public DispatchJob(
        Guid id,
        Guid haulShareId,
        Route route,
        DateTime scheduledPickupTime)
        : base(id)
    {
        if (route == null)
            throw new ArgumentNullException(nameof(route));

        Id = id; // Explicitly set for required property
        HaulShareId = haulShareId;
        Route = route;
        ScheduledPickupTime = scheduledPickupTime;
        EstimatedDeliveryTime = scheduledPickupTime.Add(route.EstimatedDuration);
        Status = DispatchJobStatus.Posted;
        CreatedAtUtc = DateTime.UtcNow;

        RaiseDomainEvent(new DispatchJobPosted(Id, HaulShareId));
    }

    public void Accept(Guid driverId)
    {
        if (Status != DispatchJobStatus.Posted)
            throw new InvalidOperationException($"Cannot accept job in status {Status}");

        AssignedDriverId = driverId;
        Status = DispatchJobStatus.Accepted;

        RaiseDomainEvent(new DispatchJobAccepted(Id, driverId));
    }

    public void RecordGeofencePing(double latitude, double longitude, DateTime pingTime)
    {
        if (Status == DispatchJobStatus.Posted)
            throw new InvalidOperationException("Cannot record ping for unaccepted job");

        CurrentLocation = new Location(latitude, longitude);

        RaiseDomainEvent(new GeofencePingReceived(Id, latitude, longitude, pingTime));
    }

    public void StartPickup()
    {
        if (Status != DispatchJobStatus.Accepted)
            throw new InvalidOperationException($"Cannot start pickup in status {Status}");

        Status = DispatchJobStatus.InProgress;
        PickupStartedAt = DateTime.UtcNow;

        RaiseDomainEvent(new PickupStarted(Id));
    }

    public void CompletePickup()
    {
        if (Status != DispatchJobStatus.InProgress || PickupStartedAt == null)
            throw new InvalidOperationException("Cannot complete pickup without starting it");

        PickupCompletedAt = DateTime.UtcNow;
        Status = DispatchJobStatus.PickupCompleted;

        RaiseDomainEvent(new PickupCompleted(Id));
    }

    public void StartDelivery()
    {
        if (Status != DispatchJobStatus.PickupCompleted)
            throw new InvalidOperationException($"Cannot start delivery in status {Status}");

        DeliveryStartedAt = DateTime.UtcNow;
        Status = DispatchJobStatus.DeliveryInProgress;

        RaiseDomainEvent(new DeliveryStarted(Id));
    }

    public void CompleteDelivery()
    {
        if (Status != DispatchJobStatus.DeliveryInProgress || DeliveryStartedAt == null)
            throw new InvalidOperationException("Cannot complete delivery without starting it");

        DeliveryCompletedAt = DateTime.UtcNow;
        Status = DispatchJobStatus.Completed;

        RaiseDomainEvent(new DeliveryCompleted(Id));
    }
}

public enum DispatchJobStatus
{
    Posted,
    Accepted,
    InProgress,
    PickupCompleted,
    DeliveryInProgress,
    Completed,
    Cancelled
}
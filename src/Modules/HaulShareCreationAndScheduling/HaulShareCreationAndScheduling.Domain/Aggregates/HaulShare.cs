using HaulShareCreationAndScheduling.Domain.Entities;
using HaulShareCreationAndScheduling.Domain.Events;
using HaulShareCreationAndScheduling.Domain.ValueObjects;
using SharedKernel.Domain;
using FarmersHaulShare.SharedKernel.Domain;

namespace HaulShareCreationAndScheduling.Domain.Aggregates;

public class HaulShare
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public Guid Id { get; private set; }
    public DateTime ScheduledPickupTime { get; private set; }
    public DeliveryWindow DeliveryWindow { get; private set; }
    public CapacityPlan CapacityPlan { get; private set; }

    // ✅ Backing field for EF Core
    private readonly List<PickupStop> _pickupStops = new();
    public IReadOnlyCollection<PickupStop> PickupStops => _pickupStops.AsReadOnly();

    private HaulShare() { } // ✅ EF Core

    public HaulShare(
        Guid id,
        IEnumerable<PickupStop> pickupStops,
        CapacityPlan capacityPlan,
        DeliveryWindow deliveryWindow,
        DateTime scheduledPickupTime)
    {
        if (!pickupStops.Any())
            throw new InvalidOperationException("HaulShare must contain at least one pickup stop.");

        Id = id;
        CapacityPlan = capacityPlan;
        DeliveryWindow = deliveryWindow;
        ScheduledPickupTime = scheduledPickupTime;

        _pickupStops.AddRange(pickupStops);

        AddDomainEvent(new HaulShareCreated(Id));
    }

    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

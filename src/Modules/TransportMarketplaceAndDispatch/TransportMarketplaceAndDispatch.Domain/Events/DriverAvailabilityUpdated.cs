using FarmersHaulShare.SharedKernel.Domain;

namespace TransportMarketplaceAndDispatch.Domain.Events;

public sealed class DriverAvailabilityUpdated : IDomainEvent
{
    public Guid DriverId { get; }
    public DateTime OccurredOn => DateTime.UtcNow;

    public DriverAvailabilityUpdated(Guid driverId)
    {
        DriverId = driverId;
    }
}
using FarmersHaulShare.SharedKernel.Domain;

namespace TransportMarketplaceAndDispatch.Domain.Events;

public sealed class DispatchJobAccepted : IDomainEvent
{
    public Guid DispatchJobId { get; }
    public Guid DriverId { get; }
    public DateTime OccurredOn => DateTime.UtcNow;

    public DispatchJobAccepted(Guid dispatchJobId, Guid driverId)
    {
        DispatchJobId = dispatchJobId;
        DriverId = driverId;
    }
}
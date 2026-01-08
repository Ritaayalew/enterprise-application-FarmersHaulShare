using FarmersHaulShare.SharedKernel.Domain;

namespace TransportMarketplaceAndDispatch.Domain.Events;

public sealed class PickupStarted : IDomainEvent
{
    public Guid DispatchJobId { get; }
    public DateTime OccurredOn => DateTime.UtcNow;

    public PickupStarted(Guid dispatchJobId)
    {
        DispatchJobId = dispatchJobId;
    }
}
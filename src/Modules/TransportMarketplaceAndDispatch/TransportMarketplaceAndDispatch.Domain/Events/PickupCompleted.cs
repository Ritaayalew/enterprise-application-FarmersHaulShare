using FarmersHaulShare.SharedKernel.Domain;

namespace TransportMarketplaceAndDispatch.Domain.Events;

public sealed class PickupCompleted : IDomainEvent
{
    public Guid DispatchJobId { get; }
    public DateTime OccurredOn => DateTime.UtcNow;

    public PickupCompleted(Guid dispatchJobId)
    {
        DispatchJobId = dispatchJobId;
    }
}
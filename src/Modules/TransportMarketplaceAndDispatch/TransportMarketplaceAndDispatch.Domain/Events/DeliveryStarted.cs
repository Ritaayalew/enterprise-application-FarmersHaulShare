using FarmersHaulShare.SharedKernel.Domain;

namespace TransportMarketplaceAndDispatch.Domain.Events;

public sealed class DeliveryStarted : IDomainEvent
{
    public Guid DispatchJobId { get; }
    public DateTime OccurredOn => DateTime.UtcNow;

    public DeliveryStarted(Guid dispatchJobId)
    {
        DispatchJobId = dispatchJobId;
    }
}
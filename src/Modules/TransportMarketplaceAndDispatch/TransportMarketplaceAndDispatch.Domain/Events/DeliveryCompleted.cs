using FarmersHaulShare.SharedKernel.Domain;

namespace TransportMarketplaceAndDispatch.Domain.Events;

public sealed class DeliveryCompleted : IDomainEvent
{
    public Guid DispatchJobId { get; }
    public DateTime OccurredOn => DateTime.UtcNow;

    public DeliveryCompleted(Guid dispatchJobId)
    {
        DispatchJobId = dispatchJobId;
    }
}
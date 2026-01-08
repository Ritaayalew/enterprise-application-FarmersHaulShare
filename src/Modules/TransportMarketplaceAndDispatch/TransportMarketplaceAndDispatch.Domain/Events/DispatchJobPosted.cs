using FarmersHaulShare.SharedKernel.Domain;

namespace TransportMarketplaceAndDispatch.Domain.Events;

public sealed class DispatchJobPosted : IDomainEvent
{
    public Guid DispatchJobId { get; }
    public Guid HaulShareId { get; }
    public DateTime OccurredOn => DateTime.UtcNow;

    public DispatchJobPosted(Guid dispatchJobId, Guid haulShareId)
    {
        DispatchJobId = dispatchJobId;
        HaulShareId = haulShareId;
    }
}
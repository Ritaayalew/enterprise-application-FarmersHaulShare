using FarmersHaulShare.SharedKernel.Domain;

namespace HaulShareCreationAndScheduling.Domain.Events;

public sealed class HaulShareCreated : IDomainEvent
{
    public Guid HaulShareId { get; }
    public DateTime OccurredOn => DateTime.UtcNow;

    public HaulShareCreated(Guid haulShareId)
    {
        HaulShareId = haulShareId;
    }
}
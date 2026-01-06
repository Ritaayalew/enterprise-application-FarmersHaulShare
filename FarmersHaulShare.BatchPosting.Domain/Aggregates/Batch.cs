using FarmersHaulShare.SharedKernel;
using FarmersHaulShare.BatchPosting.Domain.Events;

namespace FarmersHaulShare.BatchPosting.Domain.Aggregates;

public class Batch : IHaveDomainEvents
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public int Quantity { get; private set; }
    public DateTime ReadyDate { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents() => _domainEvents.Clear();

    private Batch() { } // For EF

    public Batch(int quantity, DateTime readyDate)
    {
        Quantity = quantity;
        ReadyDate = readyDate;

        _domainEvents.Add(new BatchPosted(Id, Quantity, ReadyDate));
    }
}
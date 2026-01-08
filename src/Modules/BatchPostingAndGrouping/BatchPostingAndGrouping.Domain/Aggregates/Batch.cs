using FarmersHaulShare.SharedKernel.Domain;
using FarmersHaulShare.BatchPostingAndGrouping.Domain.Events;

namespace FarmersHaulShare.BatchPostingAndGrouping.Domain.Aggregates
{
    public class Batch : IHaveDomainEvents
    {
        // Properties
        public Guid Id { get; private set; }
        public int Quantity { get; private set; }
        public DateTime ReadyDate { get; private set; }

        // Domain events list
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        public void ClearDomainEvents() => _domainEvents.Clear();

        // EF Core needs a private constructor
        private Batch() { }

        // Constructor for creating a new Batch
        public Batch(Guid id, int quantity, DateTime readyDate)
        {
            Id = id;
            Quantity = quantity;
            ReadyDate = readyDate;

            // Raise domain event
            _domainEvents.Add(new BatchPosted(id));
        }
    }
}

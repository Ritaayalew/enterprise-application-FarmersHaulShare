using System.Collections.Generic;

namespace FarmersHaulShare.SharedKernel.Domain
{
    public interface IHaveDomainEvents
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}

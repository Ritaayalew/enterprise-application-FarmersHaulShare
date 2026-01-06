using System.Collections.Generic;

namespace FarmersHaulShare.SharedKernel;

public interface IHaveDomainEvents
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}

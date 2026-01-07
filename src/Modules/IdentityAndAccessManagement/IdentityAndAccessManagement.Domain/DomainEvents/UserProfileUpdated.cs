using SharedKernel.Domain;

namespace IdentityAndAccessManagement.Domain.DomainEvents;

public sealed class UserProfileUpdated : IDomainEvent
{
    public Guid UserId { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserProfileUpdated(Guid userId)
    {
        UserId = userId;
    }
}

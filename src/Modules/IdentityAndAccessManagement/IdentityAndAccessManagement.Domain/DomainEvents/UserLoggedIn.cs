using SharedKernel.Domain;

namespace IdentityAndAccessManagement.Domain.DomainEvents;

public sealed class UserLoggedIn : IDomainEvent
{
    public Guid UserId { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserLoggedIn(Guid userId)
    {
        UserId = userId;
    }
}

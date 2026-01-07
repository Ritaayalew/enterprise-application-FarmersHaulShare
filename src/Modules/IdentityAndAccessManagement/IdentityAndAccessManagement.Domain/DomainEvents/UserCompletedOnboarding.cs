using SharedKernel.Domain;

namespace IdentityAndAccessManagement.Domain.DomainEvents;

public sealed class UserCompletedOnboarding : IDomainEvent
{
    public Guid UserId { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserCompletedOnboarding(Guid userId)
    {
        UserId = userId;
    }
}

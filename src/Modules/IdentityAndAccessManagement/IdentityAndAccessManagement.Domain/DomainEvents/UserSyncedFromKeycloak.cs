using SharedKernel.Domain;

namespace IdentityAndAccessManagement.Domain.DomainEvents;

public sealed class UserSyncedFromKeycloak : IDomainEvent
{
    public Guid UserId { get; }
    public string KeycloakSubjectId { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    public UserSyncedFromKeycloak(
        Guid userId,
        string keycloakSubjectId)
    {
        UserId = userId;
        KeycloakSubjectId = keycloakSubjectId;
    }
}

using SharedKernel.Domain;
using IdentityAndAccessManagement.Domain.Roles;
using IdentityAndAccessManagement.Domain.ValueObjects;
using IdentityAndAccessManagement.Domain.DomainEvents;

namespace IdentityAndAccessManagement.Domain.Entities;

public class User : AggregateRoot<Guid>
{
    public string KeycloakSubjectId { get; private set; } = null!;

    public Email Email { get; private set; } = null!;
    public FullName Name { get; private set; } = null!;
    public PhoneNumber? PhoneNumber { get; private set; }
    public CooperativeId? CooperativeId { get; private set; }

    public DateTime? LastLoginAt { get; private set; }
    public bool HasCompletedOnboarding { get; private set; }

    private readonly List<Role> _roles = new();
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    private User() { } // EF Core

    public static User CreateFromKeycloak(
        Guid id,
        string keycloakSubjectId,
        Email email,
        FullName name,
        IEnumerable<Role> rolesFromToken)
    {
        var user = new User
        {
            Id = id,
            KeycloakSubjectId = keycloakSubjectId,
            Email = email,
            Name = name
        };

        foreach (var role in rolesFromToken)
        {
            user._roles.Add(role);
        }

        user.RaiseDomainEvent(
            new UserSyncedFromKeycloak(user.Id, user.KeycloakSubjectId));

        return user;
    }

    public void UpdateProfile(
        PhoneNumber? phoneNumber,
        CooperativeId? cooperativeId)
    {
        PhoneNumber = phoneNumber;
        CooperativeId = cooperativeId;

        RaiseDomainEvent(new UserProfileUpdated(Id));
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }

    public void CompleteOnboarding()
    {
        HasCompletedOnboarding = true;
    }
}

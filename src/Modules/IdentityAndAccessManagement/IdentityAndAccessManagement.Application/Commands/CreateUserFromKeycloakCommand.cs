using MediatR;
using IdentityAndAccessManagement.Domain.Entities;
using IdentityAndAccessManagement.Domain.Repositories;
using IdentityAndAccessManagement.Domain.Roles;
using IdentityAndAccessManagement.Domain.ValueObjects;
using IdentityAndAccessManagement.Application.Abstractions;

namespace IdentityAndAccessManagement.Application.Commands;

public sealed record CreateUserFromKeycloakCommand(
    string KeycloakSubjectId,
    string Email,
    string FirstName,
    string LastName,
    IReadOnlyCollection<string> Roles
) : IRequest<Guid>;

internal sealed class CreateUserFromKeycloakCommandHandler
    : IRequestHandler<CreateUserFromKeycloakCommand, Guid>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public CreateUserFromKeycloakCommandHandler(
        IUserRepository users,
        IUnitOfWork uow)
    {
        _users = users;
        _uow = uow;
    }

    public async Task<Guid> Handle(
        CreateUserFromKeycloakCommand request,
        CancellationToken ct)
    {
        var existing =
            await _users.GetByKeycloakSubjectIdAsync(request.KeycloakSubjectId, ct);

        if (existing is not null)
            return existing.Id;

        var user = User.CreateFromKeycloak(
            Guid.NewGuid(),
            request.KeycloakSubjectId,
            new Email(request.Email),
            new FullName(request.FirstName, request.LastName),
            request.Roles.Select(r => new Role(r))
        );

        await _users.AddAsync(user, ct);
        await _uow.CommitAsync(ct);

        return user.Id;
    }
}

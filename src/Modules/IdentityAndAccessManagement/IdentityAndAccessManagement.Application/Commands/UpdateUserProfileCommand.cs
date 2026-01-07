using MediatR;
using IdentityAndAccessManagement.Domain.Repositories;
using IdentityAndAccessManagement.Domain.ValueObjects;
using IdentityAndAccessManagement.Application.Abstractions;

namespace IdentityAndAccessManagement.Application.Commands;

public sealed record UpdateUserProfileCommand(
    Guid UserId,
    string? PhoneNumber,
    string? CooperativeId
) : IRequest;

internal sealed class UpdateUserProfileCommandHandler
    : IRequestHandler<UpdateUserProfileCommand>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public UpdateUserProfileCommandHandler(
        IUserRepository users,
        IUnitOfWork uow)
    {
        _users = users;
        _uow = uow;
    }

    public async Task Handle(
        UpdateUserProfileCommand request,
        CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(request.UserId, ct)
            ?? throw new InvalidOperationException("User not found");

        user.UpdateProfile(
            request.PhoneNumber is null ? null : new PhoneNumber(request.PhoneNumber),
            request.CooperativeId is null ? null : new CooperativeId(request.CooperativeId)
        );

        await _uow.CommitAsync(ct);
    }
}

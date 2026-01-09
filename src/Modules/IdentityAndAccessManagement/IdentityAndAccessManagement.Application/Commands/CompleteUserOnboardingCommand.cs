using MediatR;
using IdentityAndAccessManagement.Application.Abstractions;

namespace IdentityAndAccessManagement.Application.Commands;

public sealed record CompleteUserOnboardingCommand(Guid UserId) : IRequest;

internal sealed class CompleteUserOnboardingCommandHandler
    : IRequestHandler<CompleteUserOnboardingCommand>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public CompleteUserOnboardingCommandHandler(
        IUserRepository users,
        IUnitOfWork uow)
    {
        _users = users;
        _uow = uow;
    }

    public async Task Handle(
        CompleteUserOnboardingCommand request,
        CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(request.UserId, ct)
            ?? throw new InvalidOperationException("User not found");

        user.CompleteOnboarding();
        await _uow.CommitAsync(ct);
    }
}

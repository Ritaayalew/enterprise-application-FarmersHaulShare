using MediatR;
using IdentityAndAccessManagement.Application.DTOs;
using IdentityAndAccessManagement.Application.Abstractions;
using IdentityAndAccessManagement.Application.Mapping;

namespace IdentityAndAccessManagement.Application.Queries;

public sealed record GetUserByKeycloakSubjectQuery(string SubjectId)
    : IRequest<UserDto?>;

internal sealed class GetUserByKeycloakSubjectQueryHandler
    : IRequestHandler<GetUserByKeycloakSubjectQuery, UserDto?>
{
    private readonly IUserRepository _users;

    public GetUserByKeycloakSubjectQueryHandler(IUserRepository users)
    {
        _users = users;
    }

    public async Task<UserDto?> Handle(
        GetUserByKeycloakSubjectQuery request,
        CancellationToken ct)
    {
        var user =
            await _users.GetByKeycloakSubjectIdAsync(request.SubjectId, ct);

        return user?.ToDto();
    }
}

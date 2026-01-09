using MediatR;
using IdentityAndAccessManagement.Application.DTOs;
using IdentityAndAccessManagement.Application.Abstractions;
using IdentityAndAccessManagement.Application.Mapping;

namespace IdentityAndAccessManagement.Application.Queries;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<UserDto?>;

internal sealed class GetUserByIdQueryHandler
    : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    private readonly IUserRepository _users;

    public GetUserByIdQueryHandler(IUserRepository users)
    {
        _users = users;
    }

    public async Task<UserDto?> Handle(
        GetUserByIdQuery request,
        CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(request.UserId, ct);
        return user?.ToDto();
    }
}

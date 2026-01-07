using IdentityAndAccessManagement.Application.DTOs;
using IdentityAndAccessManagement.Domain.Entities;

namespace IdentityAndAccessManagement.Application.Mapping;

public static class UserMapping
{
    public static UserDto ToDto(this User user)
        => new(
            user.Id,
            user.Email.Value,
            user.Name.ToString(),
            user.PhoneNumber?.Value,
            user.CooperativeId?.Value,
            user.Roles.Select(r => r.Value).ToList(),
            user.HasCompletedOnboarding
        );
}

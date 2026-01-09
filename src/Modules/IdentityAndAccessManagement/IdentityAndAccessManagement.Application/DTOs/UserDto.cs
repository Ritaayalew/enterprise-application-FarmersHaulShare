namespace IdentityAndAccessManagement.Application.DTOs;

public sealed record UserDto(
    Guid Id,
    string Email,
    string FullName,
    string? PhoneNumber,
    string? CooperativeId,
    IReadOnlyCollection<string> Roles,
    bool HasCompletedOnboarding
);

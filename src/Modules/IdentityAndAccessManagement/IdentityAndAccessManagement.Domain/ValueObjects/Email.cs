using System.Text.RegularExpressions;
using SharedKernel.Domain;
namespace IdentityAndAccessManagement.Domain.ValueObjects;

public sealed record Email
{
    public string Value { get; }

    private static readonly Regex EmailRegex =
        new(@"^[\w\.-]+@([\w-]+\.)+[\w-]{2,}$", RegexOptions.Compiled);

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email cannot be empty");

        value = value.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(value))
            throw new DomainException("Invalid email format");

        Value = value;
    }
}

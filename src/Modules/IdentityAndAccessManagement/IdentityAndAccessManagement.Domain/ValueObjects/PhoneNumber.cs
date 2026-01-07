using System.Text.RegularExpressions;
using SharedKernel.Domain;
namespace IdentityAndAccessManagement.Domain.ValueObjects;

public sealed record PhoneNumber
{
    public string Value { get; }

    private static readonly Regex EthiopianPhoneRegex =
        new(@"^(\+2519|09)\d{8}$", RegexOptions.Compiled);

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Phone number cannot be empty");

        value = value.Trim();

        if (!EthiopianPhoneRegex.IsMatch(value))
            throw new DomainException("Invalid Ethiopian phone number format");

        Value = value;
    }
}

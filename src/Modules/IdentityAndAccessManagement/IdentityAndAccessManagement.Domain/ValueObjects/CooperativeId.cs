namespace IdentityAndAccessManagement.Domain.ValueObjects;

using SharedKernel.Domain;

public sealed record CooperativeId
{
    public string Value { get; }

    public CooperativeId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Cooperative ID cannot be empty");

        Value = value.Trim().ToUpperInvariant();
    }
}

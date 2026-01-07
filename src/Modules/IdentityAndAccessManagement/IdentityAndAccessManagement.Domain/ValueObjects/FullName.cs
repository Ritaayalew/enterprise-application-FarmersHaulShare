namespace IdentityAndAccessManagement.Domain.ValueObjects;

public record FullName(string FirstName, string LastName)
{
    public string DisplayName => $"{FirstName} {LastName}".Trim();

    public override string ToString() => DisplayName;
}
namespace TransportMarketplaceAndDispatch.Application.Commands;

public sealed class RegisterDriverCommand
{
    public string UserId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}
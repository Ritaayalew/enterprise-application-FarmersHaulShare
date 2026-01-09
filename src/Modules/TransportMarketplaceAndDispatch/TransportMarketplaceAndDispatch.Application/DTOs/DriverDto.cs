namespace TransportMarketplaceAndDispatch.Application.DTOs;

public sealed class DriverDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public double? CurrentLatitude { get; init; }
    public double? CurrentLongitude { get; init; }
    public bool IsActive { get; init; }
    public List<VehicleDto> Vehicles { get; init; } = new();
}
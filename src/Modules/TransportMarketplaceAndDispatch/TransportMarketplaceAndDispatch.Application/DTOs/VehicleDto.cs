namespace TransportMarketplaceAndDispatch.Application.DTOs;

public sealed class VehicleDto
{
    public Guid Id { get; init; }
    public string PlateNumber { get; init; } = string.Empty;
    public string VehicleTypeName { get; init; } = string.Empty;
    public double MaxWeightKg { get; init; }
    public double MaxVolumeCubicMeters { get; init; }
    public bool IsVerified { get; init; }
}
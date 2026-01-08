namespace TransportMarketplaceAndDispatch.Application.Commands;

public sealed class AddVehicleCommand
{
    public Guid DriverId { get; init; }
    public string PlateNumber { get; init; } = string.Empty;
    public string VehicleTypeName { get; init; } = string.Empty;
    public double? MaxWeightKg { get; init; }
    public double? MaxVolumeCubicMeters { get; init; }
}
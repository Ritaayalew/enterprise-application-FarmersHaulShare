namespace TransportMarketplaceAndDispatch.Application.Commands;

public sealed class VerifyVehicleCommand
{
    public Guid DriverId { get; init; }
    public Guid VehicleId { get; init; }
}
namespace TransportMarketplaceAndDispatch.Application.Commands;

public sealed class StartPickupCommand
{
    public Guid DispatchJobId { get; init; }
    public Guid DriverId { get; init; }
}
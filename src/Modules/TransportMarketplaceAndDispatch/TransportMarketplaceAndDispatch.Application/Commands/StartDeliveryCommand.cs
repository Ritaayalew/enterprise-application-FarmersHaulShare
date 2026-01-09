namespace TransportMarketplaceAndDispatch.Application.Commands;

public sealed class StartDeliveryCommand
{
    public Guid DispatchJobId { get; init; }
    public Guid DriverId { get; init; }
}
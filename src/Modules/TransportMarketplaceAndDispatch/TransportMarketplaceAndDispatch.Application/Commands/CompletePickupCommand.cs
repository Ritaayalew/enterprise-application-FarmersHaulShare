namespace TransportMarketplaceAndDispatch.Application.Commands;

public sealed class CompletePickupCommand
{
    public Guid DispatchJobId { get; init; }
    public Guid DriverId { get; init; }
}
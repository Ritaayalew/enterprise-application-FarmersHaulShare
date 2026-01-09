namespace TransportMarketplaceAndDispatch.Application.Commands;

public sealed class CompleteDeliveryCommand
{
    public Guid DispatchJobId { get; init; }
    public Guid DriverId { get; init; }
}
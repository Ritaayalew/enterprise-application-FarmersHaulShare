namespace TransportMarketplaceAndDispatch.Application.Commands;

public sealed class AcceptDispatchJobCommand
{
    public Guid DispatchJobId { get; init; }
    public Guid DriverId { get; init; }
}
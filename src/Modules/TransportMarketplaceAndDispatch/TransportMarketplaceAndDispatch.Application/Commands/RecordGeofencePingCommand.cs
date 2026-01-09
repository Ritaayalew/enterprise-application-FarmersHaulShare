namespace TransportMarketplaceAndDispatch.Application.Commands;

public sealed class RecordGeofencePingCommand
{
    public Guid DispatchJobId { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public DateTime PingTime { get; init; }
}
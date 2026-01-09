namespace TransportMarketplaceAndDispatch.Application.Commands;

public sealed class PostDispatchJobCommand
{
    public Guid HaulShareId { get; init; }
    public double OriginLatitude { get; init; }
    public double OriginLongitude { get; init; }
    public double DestinationLatitude { get; init; }
    public double DestinationLongitude { get; init; }
    public List<PickupStopDto>? PickupStops { get; init; }
    public DateTime ScheduledPickupTime { get; init; }
}

public sealed class PickupStopDto
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}
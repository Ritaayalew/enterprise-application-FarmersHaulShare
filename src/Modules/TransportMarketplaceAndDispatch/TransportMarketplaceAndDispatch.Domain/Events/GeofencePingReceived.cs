using FarmersHaulShare.SharedKernel.Domain;

namespace TransportMarketplaceAndDispatch.Domain.Events;

public sealed class GeofencePingReceived : IDomainEvent
{
    public Guid DispatchJobId { get; }
    public double Latitude { get; }
    public double Longitude { get; }
    public DateTime PingTime { get; }
    public DateTime OccurredOn => DateTime.UtcNow;

    public GeofencePingReceived(Guid dispatchJobId, double latitude, double longitude, DateTime pingTime)
    {
        DispatchJobId = dispatchJobId;
        Latitude = latitude;
        Longitude = longitude;
        PingTime = pingTime;
    }
}
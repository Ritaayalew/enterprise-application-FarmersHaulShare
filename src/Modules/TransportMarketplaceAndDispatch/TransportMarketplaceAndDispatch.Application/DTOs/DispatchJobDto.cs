namespace TransportMarketplaceAndDispatch.Application.DTOs;

public sealed class DispatchJobDto
{
    public Guid Id { get; init; }
    public Guid HaulShareId { get; init; }
    public double OriginLatitude { get; init; }
    public double OriginLongitude { get; init; }
    public double DestinationLatitude { get; init; }
    public double DestinationLongitude { get; init; }
    public double EstimatedDistanceKm { get; init; }
    public TimeSpan EstimatedDuration { get; init; }
    public DateTime ScheduledPickupTime { get; init; }
    public DateTime EstimatedDeliveryTime { get; init; }
    public string Status { get; init; } = string.Empty;
    public Guid? AssignedDriverId { get; init; }
    public double? CurrentLatitude { get; init; }
    public double? CurrentLongitude { get; init; }
    public DateTime? PickupStartedAt { get; init; }
    public DateTime? PickupCompletedAt { get; init; }
    public DateTime? DeliveryStartedAt { get; init; }
    public DateTime? DeliveryCompletedAt { get; init; }
}
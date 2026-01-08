namespace TransportMarketplaceAndDispatch.Application.Commands;

public sealed class UpdateAvailabilityCommand
{
    public Guid DriverId { get; init; }
    public double? CurrentLatitude { get; init; }
    public double? CurrentLongitude { get; init; }
    public DateTime? AvailabilityStartTime { get; init; }
    public DateTime? AvailabilityEndTime { get; init; }
    public DayOfWeek[]? AvailableDays { get; init; }
}
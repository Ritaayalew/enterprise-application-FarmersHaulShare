using SharedKernel.Domain;

namespace TransportMarketplaceAndDispatch.Domain.ValueObjects;

public sealed class Route : ValueObject
{
    public Location Origin { get; } = null!;
    public Location Destination { get; } = null!;
    public List<Location> Waypoints { get; }
    public double EstimatedDistanceKm { get; }
    public TimeSpan EstimatedDuration { get; }

    private Route() { Waypoints = new(); } // EF Core

    public Route(Location origin, Location destination, List<Location>? waypoints = null)
    {
        Origin = origin ?? throw new ArgumentNullException(nameof(origin));
        Destination = destination ?? throw new ArgumentNullException(nameof(destination));
        Waypoints = waypoints ?? new List<Location>();

        EstimatedDistanceKm = CalculateTotalDistance();
        EstimatedDuration = CalculateEstimatedDuration();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Origin;
        yield return Destination;
        foreach (var waypoint in Waypoints)
        {
            yield return waypoint;
        }
    }

    private double CalculateTotalDistance()
    {
        double total = 0;
        var current = Origin;

        foreach (var waypoint in Waypoints)
        {
            total += current.CalculateDistance(waypoint);
            current = waypoint;
        }

        total += current.CalculateDistance(Destination);
        return total;
    }

    private TimeSpan CalculateEstimatedDuration()
    {
        // Assume average speed of 50 km/h for trucks, 30 km/h for urban areas
        const double averageSpeedKmh = 40.0;
        var hours = EstimatedDistanceKm / averageSpeedKmh;
        return TimeSpan.FromHours(hours);
    }
}
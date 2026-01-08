namespace BatchPostingAndGrouping.Domain.ValueObjects;

/// <summary>
/// Value object representing a geographic location (latitude, longitude)
/// </summary>
public sealed class Location : SharedKernel.Domain.ValueObject
{
    public double Latitude { get; private init; }
    public double Longitude { get; private init; }
    public string? Address { get; private init; }

    private Location() { } // EF Core

    public Location(double latitude, double longitude, string? address = null)
    {
        if (latitude < -90 || latitude > 90)
            throw new SharedKernel.Domain.DomainException("Latitude must be between -90 and 90 degrees.");
        if (longitude < -180 || longitude > 180)
            throw new SharedKernel.Domain.DomainException("Longitude must be between -180 and 180 degrees.");

        Latitude = latitude;
        Longitude = longitude;
        Address = address;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Math.Round(Latitude, 6); // Round to 6 decimal places for comparison
        yield return Math.Round(Longitude, 6);
    }

    /// <summary>
    /// Calculates distance in kilometers between two locations using Haversine formula
    /// </summary>
    public double DistanceTo(Location other)
    {
        const double earthRadiusKm = 6371.0;
        
        var dLat = ToRadians(other.Latitude - Latitude);
        var dLon = ToRadians(other.Longitude - Longitude);
        
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(Latitude)) * Math.Cos(ToRadians(other.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        
        return earthRadiusKm * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;

    public override string ToString() => $"({Latitude:F6}, {Longitude:F6})";
}

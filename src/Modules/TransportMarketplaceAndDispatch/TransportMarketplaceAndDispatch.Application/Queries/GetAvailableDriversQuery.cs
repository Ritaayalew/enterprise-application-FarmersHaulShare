using TransportMarketplaceAndDispatch.Application.DTOs;

namespace TransportMarketplaceAndDispatch.Application.Queries;

public sealed class GetAvailableDriversQuery
{
    public double? NearLatitude { get; init; }
    public double? NearLongitude { get; init; }
    public double? WithinRadiusKm { get; init; }
}

public sealed class GetAvailableDriversResult
{
    public List<DriverDto> Drivers { get; init; } = new();
}
using TransportMarketplaceAndDispatch.Application.DTOs;
using TransportMarketplaceAndDispatch.Application.Queries;
using TransportMarketplaceAndDispatch.Domain.Repositories;

namespace TransportMarketplaceAndDispatch.Application.Handlers;

public sealed class GetAvailableDriversQueryHandler
{
    private readonly IDriverRepository _driverRepository;

    public GetAvailableDriversQueryHandler(IDriverRepository driverRepository)
    {
        _driverRepository = driverRepository;
    }

    public async Task<GetAvailableDriversResult> Handle(GetAvailableDriversQuery query, CancellationToken cancellationToken)
    {
        var drivers = await _driverRepository.GetAvailableDriversAsync(cancellationToken);

        // Filter by location if provided
        if (query.NearLatitude.HasValue && query.NearLongitude.HasValue && query.WithinRadiusKm.HasValue)
        {
            var filterLocation = new Domain.ValueObjects.Location(query.NearLatitude.Value, query.NearLongitude.Value);
            drivers = drivers.Where(d =>
            {
                if (d.CurrentLocation == null) return false;
                var distance = filterLocation.CalculateDistance(d.CurrentLocation);
                return distance <= query.WithinRadiusKm.Value;
            }).ToList();
        }

        var driverDtos = drivers.Select(d => new DriverDto
        {
            Id = d.Id,
            UserId = d.UserId,
            Name = d.Name,
            PhoneNumber = d.PhoneNumber,
            CurrentLatitude = d.CurrentLocation?.Latitude,
            CurrentLongitude = d.CurrentLocation?.Longitude,
            IsActive = d.IsActive,
            Vehicles = d.Vehicles.Select(v => new VehicleDto
            {
                Id = v.Id,
                PlateNumber = v.PlateNumber,
                VehicleTypeName = v.Type.Name,
                MaxWeightKg = v.Type.MaxWeightKg,
                MaxVolumeCubicMeters = v.Type.MaxVolumeCubicMeters,
                IsVerified = v.IsVerified
            }).ToList()
        }).ToList();

        return new GetAvailableDriversResult { Drivers = driverDtos };
    }
}
using TransportMarketplaceAndDispatch.Application.DTOs;
using TransportMarketplaceAndDispatch.Application.Queries;
using TransportMarketplaceAndDispatch.Domain.Repositories;

namespace TransportMarketplaceAndDispatch.Application.Handlers;

public sealed class GetAvailableJobsQueryHandler
{
    private readonly IDispatchJobRepository _dispatchJobRepository;

    public GetAvailableJobsQueryHandler(IDispatchJobRepository dispatchJobRepository)
    {
        _dispatchJobRepository = dispatchJobRepository;
    }

    public async Task<GetAvailableJobsResult> Handle(GetAvailableJobsQuery query, CancellationToken cancellationToken)
    {
        var jobs = await _dispatchJobRepository.GetAvailableJobsAsync(cancellationToken);

        var jobDtos = jobs.Select(j => new DispatchJobDto
        {
            Id = j.Id,
            HaulShareId = j.HaulShareId,
            OriginLatitude = j.Route.Origin.Latitude,
            OriginLongitude = j.Route.Origin.Longitude,
            DestinationLatitude = j.Route.Destination.Latitude,
            DestinationLongitude = j.Route.Destination.Longitude,
            EstimatedDistanceKm = j.Route.EstimatedDistanceKm,
            EstimatedDuration = j.Route.EstimatedDuration,
            ScheduledPickupTime = j.ScheduledPickupTime,
            EstimatedDeliveryTime = j.EstimatedDeliveryTime,
            Status = j.Status.ToString(),
            AssignedDriverId = j.AssignedDriverId,
            CurrentLatitude = j.CurrentLocation?.Latitude,
            CurrentLongitude = j.CurrentLocation?.Longitude,
            PickupStartedAt = j.PickupStartedAt,
            PickupCompletedAt = j.PickupCompletedAt,
            DeliveryStartedAt = j.DeliveryStartedAt,
            DeliveryCompletedAt = j.DeliveryCompletedAt
        }).ToList();

        return new GetAvailableJobsResult { Jobs = jobDtos };
    }
}

public sealed class GetAvailableJobsResult
{
    public List<DispatchJobDto> Jobs { get; init; } = new();
}
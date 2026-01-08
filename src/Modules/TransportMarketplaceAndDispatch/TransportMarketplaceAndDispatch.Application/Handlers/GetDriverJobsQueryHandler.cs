using TransportMarketplaceAndDispatch.Application.DTOs;
using TransportMarketplaceAndDispatch.Application.Queries;
using TransportMarketplaceAndDispatch.Domain.Repositories;

namespace TransportMarketplaceAndDispatch.Application.Handlers;

public sealed class GetDriverJobsQueryHandler
{
    private readonly IDispatchJobRepository _dispatchJobRepository;

    public GetDriverJobsQueryHandler(IDispatchJobRepository dispatchJobRepository)
    {
        _dispatchJobRepository = dispatchJobRepository;
    }

    public async Task<GetDriverJobsResult> Handle(GetDriverJobsQuery query, CancellationToken cancellationToken)
    {
        var jobs = await _dispatchJobRepository.GetByDriverIdAsync(query.DriverId, cancellationToken);

        if (!query.IncludeCompleted)
        {
            jobs = jobs.Where(j => j.Status != Domain.Aggregates.DispatchJobStatus.Completed &&
                                   j.Status != Domain.Aggregates.DispatchJobStatus.Cancelled).ToList();
        }

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

        return new GetDriverJobsResult { Jobs = jobDtos };
    }
}
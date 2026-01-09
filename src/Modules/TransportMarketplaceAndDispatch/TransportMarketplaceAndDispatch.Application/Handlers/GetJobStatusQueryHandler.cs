using TransportMarketplaceAndDispatch.Application.DTOs;
using TransportMarketplaceAndDispatch.Application.Queries;
using TransportMarketplaceAndDispatch.Domain.Repositories;

namespace TransportMarketplaceAndDispatch.Application.Handlers;

public sealed class GetJobStatusQueryHandler
{
    private readonly IDispatchJobRepository _dispatchJobRepository;

    public GetJobStatusQueryHandler(IDispatchJobRepository dispatchJobRepository)
    {
        _dispatchJobRepository = dispatchJobRepository;
    }

    public async Task<GetJobStatusResult> Handle(GetJobStatusQuery query, CancellationToken cancellationToken)
    {
        var job = await _dispatchJobRepository.GetByIdAsync(query.DispatchJobId, cancellationToken);

        if (job == null)
            return new GetJobStatusResult { Job = null };

        var jobDto = new DispatchJobDto
        {
            Id = job.Id,
            HaulShareId = job.HaulShareId,
            OriginLatitude = job.Route.Origin.Latitude,
            OriginLongitude = job.Route.Origin.Longitude,
            DestinationLatitude = job.Route.Destination.Latitude,
            DestinationLongitude = job.Route.Destination.Longitude,
            EstimatedDistanceKm = job.Route.EstimatedDistanceKm,
            EstimatedDuration = job.Route.EstimatedDuration,
            ScheduledPickupTime = job.ScheduledPickupTime,
            EstimatedDeliveryTime = job.EstimatedDeliveryTime,
            Status = job.Status.ToString(),
            AssignedDriverId = job.AssignedDriverId,
            CurrentLatitude = job.CurrentLocation?.Latitude,
            CurrentLongitude = job.CurrentLocation?.Longitude,
            PickupStartedAt = job.PickupStartedAt,
            PickupCompletedAt = job.PickupCompletedAt,
            DeliveryStartedAt = job.DeliveryStartedAt,
            DeliveryCompletedAt = job.DeliveryCompletedAt
        };

        return new GetJobStatusResult { Job = jobDto };
    }
}
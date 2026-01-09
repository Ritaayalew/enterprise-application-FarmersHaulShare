using TransportMarketplaceAndDispatch.Application.Commands;
using TransportMarketplaceAndDispatch.Domain.Aggregates;
using TransportMarketplaceAndDispatch.Domain.Repositories;
using TransportMarketplaceAndDispatch.Domain.ValueObjects;

namespace TransportMarketplaceAndDispatch.Application.Handlers;

public sealed class PostDispatchJobHandler
{
    private readonly IDispatchJobRepository _dispatchJobRepository;

    public PostDispatchJobHandler(IDispatchJobRepository dispatchJobRepository)
    {
        _dispatchJobRepository = dispatchJobRepository;
    }

    public async Task<Guid> Handle(PostDispatchJobCommand command, CancellationToken cancellationToken)
    {
        // Check if job already exists for this haul share
        var existingJob = await _dispatchJobRepository.GetByHaulShareIdAsync(command.HaulShareId, cancellationToken);
        if (existingJob != null)
            throw new InvalidOperationException($"Dispatch job for HaulShare {command.HaulShareId} already exists");

        var origin = new Location(command.OriginLatitude, command.OriginLongitude);
        var destination = new Location(command.DestinationLatitude, command.DestinationLongitude);

        List<Location>? waypoints = null;
        if (command.PickupStops != null && command.PickupStops.Any())
        {
            waypoints = command.PickupStops
                .Select(s => new Location(s.Latitude, s.Longitude))
                .ToList();
        }

        var route = new Route(origin, destination, waypoints);

        var dispatchJob = new DispatchJob(
            Guid.NewGuid(),
            command.HaulShareId,
            route,
            command.ScheduledPickupTime
        );

        await _dispatchJobRepository.AddAsync(dispatchJob, cancellationToken);
        await _dispatchJobRepository.SaveChangesAsync(cancellationToken);

        return dispatchJob.Id;
    }
}
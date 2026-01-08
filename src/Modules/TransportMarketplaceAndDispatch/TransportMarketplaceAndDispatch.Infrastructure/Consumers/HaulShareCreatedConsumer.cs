using HaulShareCreationAndScheduling.Domain.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using TransportMarketplaceAndDispatch.Application.Commands;
using TransportMarketplaceAndDispatch.Application.Handlers;

namespace TransportMarketplaceAndDispatch.Infrastructure.Consumers;

/// <summary>
/// MassTransit consumer for HaulShareCreated event
/// Automatically creates a dispatch job when a haul share is created
/// 
/// Note: This consumer requires the HaulShare details to create a proper dispatch job.
/// In a production system, you would either:
/// 1. Include more data in the HaulShareCreated event (recommended for better decoupling)
/// 2. Query a read model/projection that contains the necessary route information
/// 3. Call an API endpoint to get the haul share details
/// 
/// For this implementation, we'll need to enhance the HaulShareCreated event to include
/// route information, or create a separate event handler that queries the haul share.
/// </summary>
public class HaulShareCreatedConsumer : IConsumer<HaulShareCreated>
{
    private readonly PostDispatchJobHandler _postDispatchJobHandler;
    private readonly ILogger<HaulShareCreatedConsumer> _logger;

    public HaulShareCreatedConsumer(
        PostDispatchJobHandler postDispatchJobHandler,
        ILogger<HaulShareCreatedConsumer> logger)
    {
        _postDispatchJobHandler = postDispatchJobHandler ?? throw new ArgumentNullException(nameof(postDispatchJobHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<HaulShareCreated> context)
    {
        _logger.LogInformation("Received HaulShareCreated event for HaulShareId: {HaulShareId}", context.Message.HaulShareId);

        try
        {
            // TODO: This implementation needs enhancement to work properly.
            // The HaulShareCreated event currently only contains the HaulShareId.
            // To create a dispatch job, we need:
            // - Origin location (first pickup stop or a specified origin)
            // - Destination location (from DeliveryWindow or a specified destination)
            // - Pickup stops (as waypoints)
            // - Scheduled pickup time
            
            // Option 1: Enhance the event to include route data (recommended)
            // Option 2: Query a read model that has this information
            // Option 3: Make an API call to get haul share details
            
            // For now, logging that the event was received
            // When the event is enhanced or a read model is available, uncomment and complete:
            
            /*
            var command = new PostDispatchJobCommand
            {
                HaulShareId = context.Message.HaulShareId,
                OriginLatitude = originLat,
                OriginLongitude = originLon,
                DestinationLatitude = destLat,
                DestinationLongitude = destLon,
                PickupStops = pickupStops.Select(ps => new PickupStopDto 
                { 
                    Latitude = ps.Latitude, 
                    Longitude = ps.Longitude 
                }).ToList(),
                ScheduledPickupTime = scheduledPickupTime
            };
            
            await _postDispatchJobHandler.Handle(command, context.CancellationToken);
            */

            _logger.LogInformation(
                "HaulShareCreated event received. Dispatch job creation pending implementation of " +
                "route data retrieval (either via enhanced event or read model query) for HaulShareId: {HaulShareId}", 
                context.Message.HaulShareId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing HaulShareCreated event for HaulShareId: {HaulShareId}", context.Message.HaulShareId);
            throw;
        }
    }
}
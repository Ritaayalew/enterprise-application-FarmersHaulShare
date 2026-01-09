using MassTransit;
using MessagingAndNotifications.Application.EventHandlers;
using TransportMarketplaceAndDispatch.Domain.Events;
using Microsoft.Extensions.Logging;

namespace MessagingAndNotifications.Infrastructure.Consumers;

/// <summary>
/// MassTransit consumers for transport status update events
/// </summary>
public class PickupStartedConsumer : IConsumer<PickupStarted>
{
    private readonly IStatusUpdateEventHandler _eventHandler;
    private readonly ILogger<PickupStartedConsumer> _logger;

    public PickupStartedConsumer(
        IStatusUpdateEventHandler eventHandler,
        ILogger<PickupStartedConsumer> logger)
    {
        _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<PickupStarted> context)
    {
        _logger.LogInformation("Received PickupStarted event for DispatchJob {DispatchJobId}: {MessageId}", 
            context.Message.DispatchJobId, context.MessageId);
        try
        {
            await _eventHandler.HandlePickupStartedAsync(context.Message, context.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PickupStarted event: {MessageId}", context.MessageId);
            throw;
        }
    }
}

public class PickupCompletedConsumer : IConsumer<PickupCompleted>
{
    private readonly IStatusUpdateEventHandler _eventHandler;
    private readonly ILogger<PickupCompletedConsumer> _logger;

    public PickupCompletedConsumer(
        IStatusUpdateEventHandler eventHandler,
        ILogger<PickupCompletedConsumer> logger)
    {
        _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<PickupCompleted> context)
    {
        _logger.LogInformation("Received PickupCompleted event for DispatchJob {DispatchJobId}: {MessageId}", 
            context.Message.DispatchJobId, context.MessageId);
        try
        {
            await _eventHandler.HandlePickupCompletedAsync(context.Message, context.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PickupCompleted event: {MessageId}", context.MessageId);
            throw;
        }
    }
}

public class DeliveryStartedConsumer : IConsumer<DeliveryStarted>
{
    private readonly IStatusUpdateEventHandler _eventHandler;
    private readonly ILogger<DeliveryStartedConsumer> _logger;

    public DeliveryStartedConsumer(
        IStatusUpdateEventHandler eventHandler,
        ILogger<DeliveryStartedConsumer> logger)
    {
        _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<DeliveryStarted> context)
    {
        _logger.LogInformation("Received DeliveryStarted event for DispatchJob {DispatchJobId}: {MessageId}", 
            context.Message.DispatchJobId, context.MessageId);
        try
        {
            await _eventHandler.HandleDeliveryStartedAsync(context.Message, context.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing DeliveryStarted event: {MessageId}", context.MessageId);
            throw;
        }
    }
}

public class DeliveryCompletedConsumer : IConsumer<DeliveryCompleted>
{
    private readonly IStatusUpdateEventHandler _eventHandler;
    private readonly ILogger<DeliveryCompletedConsumer> _logger;

    public DeliveryCompletedConsumer(
        IStatusUpdateEventHandler eventHandler,
        ILogger<DeliveryCompletedConsumer> logger)
    {
        _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<DeliveryCompleted> context)
    {
        _logger.LogInformation("Received DeliveryCompleted event for DispatchJob {DispatchJobId}: {MessageId}", 
            context.Message.DispatchJobId, context.MessageId);
        try
        {
            await _eventHandler.HandleDeliveryCompletedAsync(context.Message, context.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing DeliveryCompleted event: {MessageId}", context.MessageId);
            throw;
        }
    }
}

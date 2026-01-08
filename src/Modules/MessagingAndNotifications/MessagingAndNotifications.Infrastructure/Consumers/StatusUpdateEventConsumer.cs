using MassTransit;
using MessagingAndNotifications.Application.EventHandlers;
using Microsoft.Extensions.Logging;

namespace MessagingAndNotifications.Infrastructure.Consumers;

/// <summary>
/// MassTransit consumer for status update events (PickupStarted, PickupCompleted, DeliveryStarted, DeliveryCompleted)
/// Note: These are placeholders. When Transport module is implemented, replace 'object' with the actual event types
/// </summary>
public class PickupStartedConsumer : IConsumer<object> // TODO: Replace with PickupStarted when Transport module is ready
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

    public async Task Consume(ConsumeContext<object> context)
    {
        _logger.LogInformation("Received PickupStarted event: {MessageId}", context.MessageId);
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

public class PickupCompletedConsumer : IConsumer<object> // TODO: Replace with PickupCompleted when Transport module is ready
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

    public async Task Consume(ConsumeContext<object> context)
    {
        _logger.LogInformation("Received PickupCompleted event: {MessageId}", context.MessageId);
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

public class DeliveryStartedConsumer : IConsumer<object> // TODO: Replace with DeliveryStarted when Transport module is ready
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

    public async Task Consume(ConsumeContext<object> context)
    {
        _logger.LogInformation("Received DeliveryStarted event: {MessageId}", context.MessageId);
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

public class DeliveryCompletedConsumer : IConsumer<object> // TODO: Replace with DeliveryCompleted when Transport module is ready
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

    public async Task Consume(ConsumeContext<object> context)
    {
        _logger.LogInformation("Received DeliveryCompleted event: {MessageId}", context.MessageId);
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

using MassTransit;
using MessagingAndNotifications.Application.EventHandlers;
using Microsoft.Extensions.Logging;

namespace MessagingAndNotifications.Infrastructure.Consumers;

/// <summary>
/// MassTransit consumer for TransparencyReceiptGenerated event
/// Note: This is a placeholder. When Pricing module is implemented, replace 'object' with the actual event type
/// </summary>
public class ReceiptEventConsumer : IConsumer<object> // TODO: Replace with TransparencyReceiptGenerated when Pricing module is ready
{
    private readonly IReceiptEventHandler _eventHandler;
    private readonly ILogger<ReceiptEventConsumer> _logger;

    public ReceiptEventConsumer(
        IReceiptEventHandler eventHandler,
        ILogger<ReceiptEventConsumer> logger)
    {
        _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<object> context)
    {
        _logger.LogInformation("Received receipt event: {MessageId}", context.MessageId);

        try
        {
            await _eventHandler.HandleTransparencyReceiptGeneratedAsync(context.Message, context.CancellationToken);
            _logger.LogInformation("Successfully processed receipt event: {MessageId}", context.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing receipt event: {MessageId}", context.MessageId);
            throw;
        }
    }
}

using MassTransit;
using MessagingAndNotifications.Application.EventHandlers;
using Microsoft.Extensions.Logging;

namespace MessagingAndNotifications.Infrastructure.Consumers;

/// <summary>
/// MassTransit consumer for FixedPriceQuoteCalculated event
/// Note: This is a placeholder. When Pricing module is implemented, replace 'object' with the actual event type
/// </summary>
public class QuoteEventConsumer : IConsumer<object> 
{
    private readonly IQuoteEventHandler _eventHandler;
    private readonly ILogger<QuoteEventConsumer> _logger;

    public QuoteEventConsumer(
        IQuoteEventHandler eventHandler,
        ILogger<QuoteEventConsumer> logger)
    {
        _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<object> context)
    {
        _logger.LogInformation("Received quote event: {MessageId}", context.MessageId);

        try
        {
            await _eventHandler.HandleFixedPriceQuoteCalculatedAsync(context.Message, context.CancellationToken);
            _logger.LogInformation("Successfully processed quote event: {MessageId}", context.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing quote event: {MessageId}", context.MessageId);
            throw;
        }
    }
}

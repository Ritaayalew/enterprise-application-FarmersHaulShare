using MassTransit;
using MessagingAndNotifications.Application.EventHandlers;
using PricingAndFairCostSplit.Domain.Events;
using Microsoft.Extensions.Logging;

namespace MessagingAndNotifications.Infrastructure.Consumers;

/// <summary>
/// MassTransit consumer for PriceCalculated event from Pricing module
/// </summary>
public class QuoteEventConsumer : IConsumer<PriceCalculated>
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

    public async Task Consume(ConsumeContext<PriceCalculated> context)
    {
        _logger.LogInformation("Received PriceCalculated event for HaulShare {HaulShareId}: {MessageId}", 
            context.Message.HaulShareId, context.MessageId);

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

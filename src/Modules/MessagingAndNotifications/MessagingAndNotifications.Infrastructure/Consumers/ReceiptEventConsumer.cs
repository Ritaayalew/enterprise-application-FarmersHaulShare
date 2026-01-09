using MassTransit;
using MessagingAndNotifications.Application.EventHandlers;
using PricingAndFairCostSplit.Domain.Events;
using Microsoft.Extensions.Logging;

namespace MessagingAndNotifications.Infrastructure.Consumers;

/// <summary>
/// MassTransit consumer for FairCostSplitDetermined event from Pricing module
/// </summary>
public class ReceiptEventConsumer : IConsumer<FairCostSplitDetermined>
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

    public async Task Consume(ConsumeContext<FairCostSplitDetermined> context)
    {
        _logger.LogInformation("Received FairCostSplitDetermined event for HaulShare {HaulShareId}: {MessageId}", 
            context.Message.HaulShareId, context.MessageId);

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

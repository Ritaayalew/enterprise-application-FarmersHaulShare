using PricingAndFairCostSplit.Domain.Events;

namespace MessagingAndNotifications.Application.EventHandlers;

/// <summary>
/// Interface for handling quote-related events from Pricing module
/// </summary>
public interface IQuoteEventHandler
{
    /// <summary>
    /// Handles PriceCalculated event and sends quote notifications
    /// </summary>
    Task HandleFixedPriceQuoteCalculatedAsync(PriceCalculated priceCalculatedEvent, CancellationToken cancellationToken = default);
}

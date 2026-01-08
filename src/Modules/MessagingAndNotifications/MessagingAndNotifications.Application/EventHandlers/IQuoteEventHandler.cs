namespace MessagingAndNotifications.Application.EventHandlers;

/// <summary>
/// Interface for handling quote-related events from Pricing module
/// </summary>
public interface IQuoteEventHandler
{
    /// <summary>
    /// Handles FixedPriceQuoteCalculated event and sends quote notifications
    /// This will be implemented as a MassTransit consumer when Pricing module is ready
    /// </summary>
    Task HandleFixedPriceQuoteCalculatedAsync(object fixedPriceQuoteCalculatedEvent, CancellationToken cancellationToken = default);
}

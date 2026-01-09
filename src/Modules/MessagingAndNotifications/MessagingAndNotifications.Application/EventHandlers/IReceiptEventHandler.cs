using PricingAndFairCostSplit.Domain.Events;

namespace MessagingAndNotifications.Application.EventHandlers;

/// <summary>
/// Interface for handling receipt-related events from Pricing module
/// </summary>
public interface IReceiptEventHandler
{
    /// <summary>
    /// Handles FairCostSplitDetermined event and sends receipt notifications
    /// </summary>
    Task HandleTransparencyReceiptGeneratedAsync(FairCostSplitDetermined fairCostSplitEvent, CancellationToken cancellationToken = default);
}

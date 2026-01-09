using TransportMarketplaceAndDispatch.Domain.Events;

namespace MessagingAndNotifications.Application.EventHandlers;


public interface IStatusUpdateEventHandler
{
    Task HandlePickupStartedAsync(PickupStarted pickupStartedEvent, CancellationToken cancellationToken = default);

    Task HandlePickupCompletedAsync(PickupCompleted pickupCompletedEvent, CancellationToken cancellationToken = default);
    Task HandleDeliveryStartedAsync(DeliveryStarted deliveryStartedEvent, CancellationToken cancellationToken = default);
    Task HandleDeliveryCompletedAsync(DeliveryCompleted deliveryCompletedEvent, CancellationToken cancellationToken = default);
}

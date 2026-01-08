namespace MessagingAndNotifications.Application.EventHandlers;

/// <summary>
/// Interface for handling status update events from Transport/Dispatch module
/// </summary>
public interface IStatusUpdateEventHandler
{
    /// <summary>
    /// Handles PickupStarted event and sends status update notifications
    /// </summary>
    Task HandlePickupStartedAsync(object pickupStartedEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handles PickupCompleted event and sends status update notifications
    /// </summary>
    Task HandlePickupCompletedAsync(object pickupCompletedEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handles DeliveryStarted event and sends status update notifications
    /// </summary>
    Task HandleDeliveryStartedAsync(object deliveryStartedEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handles DeliveryCompleted event and sends status update notifications
    /// </summary>
    Task HandleDeliveryCompletedAsync(object deliveryCompletedEvent, CancellationToken cancellationToken = default);
}

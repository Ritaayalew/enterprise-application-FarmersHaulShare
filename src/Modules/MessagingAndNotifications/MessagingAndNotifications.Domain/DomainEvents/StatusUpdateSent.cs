using FarmersHaulShare.SharedKernel.Domain;

namespace MessagingAndNotifications.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a status update notification is sent
/// </summary>
public sealed record StatusUpdateSent : IDomainEvent
{
    public Guid NotificationId { get; init; }
    public Guid HaulShareId { get; init; }
    public Guid RecipientId { get; init; }
    public string RecipientType { get; init; } = string.Empty;
    public string StatusType { get; init; } = string.Empty; // "PickupStarted", "PickupCompleted", "DeliveryStarted", "DeliveryCompleted"
    public string? Location { get; init; }
    public DateTime? EstimatedTimeOfArrival { get; init; }
    public DateTime OccurredOn { get; init; }

    private StatusUpdateSent() { }

    public StatusUpdateSent(
        Guid notificationId,
        Guid haulShareId,
        Guid recipientId,
        string recipientType,
        string statusType,
        string? location = null,
        DateTime? estimatedTimeOfArrival = null)
    {
        if (string.IsNullOrWhiteSpace(recipientType))
            throw new ArgumentException("Recipient type cannot be empty.", nameof(recipientType));
        if (string.IsNullOrWhiteSpace(statusType))
            throw new ArgumentException("Status type cannot be empty.", nameof(statusType));

        var validStatusTypes = new[] { "PickupStarted", "PickupCompleted", "DeliveryStarted", "DeliveryCompleted" };
        if (!validStatusTypes.Contains(statusType, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException($"Invalid status type: {statusType}", nameof(statusType));

        NotificationId = notificationId;
        HaulShareId = haulShareId;
        RecipientId = recipientId;
        RecipientType = recipientType;
        StatusType = statusType;
        Location = location;
        EstimatedTimeOfArrival = estimatedTimeOfArrival;
        OccurredOn = DateTime.UtcNow;
    }
}

using MessagingAndNotifications.Domain.ValueObjects;
using SharedKernel.Domain;
using System.Diagnostics.CodeAnalysis;

namespace MessagingAndNotifications.Domain.Entities;

/// <summary>
/// Entity representing a notification sent to a user
/// </summary>
public sealed class Notification : Entity<Guid>
{
    public Guid RecipientId { get; private init; }
    public string RecipientType { get; private init; } = string.Empty; // "Farmer", "Buyer", "Driver", "Coordinator"
    public NotificationChannel Channel { get; private set; } = null!;
    public NotificationTemplate Template { get; private set; } = null!;
    public string Subject { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public NotificationStatus Status { get; private set; } = null!;
    public string NotificationType { get; private init; } = string.Empty; // "Quote", "StatusUpdate", "Receipt", etc.
    public Guid? RelatedEntityId { get; private init; } // e.g., HaulShareId, BatchId, etc.
    public string? RelatedEntityType { get; private init; } // e.g., "HaulShare", "Batch", etc.
    public Dictionary<string, string>? Metadata { get; private set; } // Additional context data
    public DateTime CreatedAtUtc { get; private init; }
    public DateTime? SentAtUtc { get; private set; }
    public DateTime? DeliveredAtUtc { get; private set; }

    private Notification() { }

    [SetsRequiredMembers]
    public Notification(
        Guid id,
        Guid recipientId,
        string recipientType,
        NotificationChannel channel,
        NotificationTemplate template,
        string notificationType,
        Guid? relatedEntityId = null,
        string? relatedEntityType = null,
        Dictionary<string, string>? metadata = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(recipientType))
            throw new SharedKernel.Domain.DomainException("Recipient type cannot be empty.");
        if (string.IsNullOrWhiteSpace(notificationType))
            throw new SharedKernel.Domain.DomainException("Notification type cannot be empty.");

        var validRecipientTypes = new[] { "Farmer", "Buyer", "Driver", "Coordinator" };
        if (!validRecipientTypes.Contains(recipientType, StringComparer.OrdinalIgnoreCase))
            throw new SharedKernel.Domain.DomainException($"Invalid recipient type: {recipientType}");

        Id = id;
        RecipientId = recipientId;
        RecipientType = recipientType.Trim();
        Channel = channel ?? throw new ArgumentNullException(nameof(channel));
        Template = template ?? throw new ArgumentNullException(nameof(template));
        NotificationType = notificationType.Trim();
        RelatedEntityId = relatedEntityId;
        RelatedEntityType = relatedEntityType?.Trim();
        Metadata = metadata;
        Status = NotificationStatus.Pending();
        CreatedAtUtc = DateTime.UtcNow;

        // Render template with metadata as placeholders
        var placeholders = metadata ?? new Dictionary<string, string>();
        Subject = template.RenderSubject(placeholders);
        Body = template.Render(placeholders);
    }

    public void MarkAsSent()
    {
        if (Status.IsSent || Status.IsDelivered)
            throw new SharedKernel.Domain.DomainException("Notification is already sent or delivered.");

        Status = NotificationStatus.Sent(DateTime.UtcNow);
        SentAtUtc = DateTime.UtcNow;
    }

    public void MarkAsDelivered()
    {
        if (!Status.IsSent)
            throw new SharedKernel.Domain.DomainException("Notification must be sent before it can be marked as delivered.");

        Status = NotificationStatus.Delivered(DateTime.UtcNow);
        DeliveredAtUtc = DateTime.UtcNow;
    }

    public void MarkAsFailed(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new SharedKernel.Domain.DomainException("Failure reason cannot be empty.");

        Status = NotificationStatus.Failed(reason, DateTime.UtcNow);
    }

    public void Cancel()
    {
        if (Status.IsSent || Status.IsDelivered)
            throw new SharedKernel.Domain.DomainException("Cannot cancel a notification that has been sent or delivered.");

        Status = NotificationStatus.Cancelled(DateTime.UtcNow);
    }
}

using FarmersHaulShare.SharedKernel.Domain;

namespace MessagingAndNotifications.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a receipt notification is sent
/// </summary>
public sealed record ReceiptSent : IDomainEvent
{
    public Guid NotificationId { get; init; }
    public Guid HaulShareId { get; init; }
    public Guid RecipientId { get; init; }
    public string RecipientType { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public decimal RecipientShare { get; init; }
    public string Currency { get; init; } = string.Empty;
    public DateTime OccurredOn { get; init; }

    private ReceiptSent() { }

    public ReceiptSent(
        Guid notificationId,
        Guid haulShareId,
        Guid recipientId,
        string recipientType,
        decimal totalAmount,
        decimal recipientShare,
        string currency = "ETB")
    {
        if (string.IsNullOrWhiteSpace(recipientType))
            throw new ArgumentException("Recipient type cannot be empty.", nameof(recipientType));

        NotificationId = notificationId;
        HaulShareId = haulShareId;
        RecipientId = recipientId;
        RecipientType = recipientType;
        TotalAmount = totalAmount;
        RecipientShare = recipientShare;
        Currency = currency;
        OccurredOn = DateTime.UtcNow;
    }
}

using FarmersHaulShare.SharedKernel.Domain;

namespace MessagingAndNotifications.Domain.DomainEvents;

/// <summary>
/// Domain event raised when a quote notification is sent to recipients
/// </summary>
public sealed record QuoteSent : IDomainEvent
{
    public Guid NotificationId { get; init; }
    public Guid HaulShareId { get; init; }
    public Guid RecipientId { get; init; }
    public string RecipientType { get; init; } = string.Empty;
    public decimal QuoteAmount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public DateTime OccurredOn { get; init; }

    private QuoteSent() { }

    public QuoteSent(
        Guid notificationId,
        Guid haulShareId,
        Guid recipientId,
        string recipientType,
        decimal quoteAmount,
        string currency = "ETB")
    {
        if (string.IsNullOrWhiteSpace(recipientType))
            throw new ArgumentException("Recipient type cannot be empty.", nameof(recipientType));

        NotificationId = notificationId;
        HaulShareId = haulShareId;
        RecipientId = recipientId;
        RecipientType = recipientType;
        QuoteAmount = quoteAmount;
        Currency = currency;
        OccurredOn = DateTime.UtcNow;
    }
}

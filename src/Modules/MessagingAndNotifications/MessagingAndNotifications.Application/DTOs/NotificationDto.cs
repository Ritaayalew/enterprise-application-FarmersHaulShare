namespace MessagingAndNotifications.Application.DTOs;

/// <summary>
/// DTO for notification response
/// </summary>
public sealed record NotificationDto
{
    public Guid Id { get; init; }
    public Guid RecipientId { get; init; }
    public string RecipientType { get; init; } = string.Empty;
    public string ChannelType { get; init; } = string.Empty;
    public string? ChannelAddress { get; init; }
    public string Subject { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string NotificationType { get; init; } = string.Empty;
    public Guid? RelatedEntityId { get; init; }
    public string? RelatedEntityType { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? SentAtUtc { get; init; }
    public DateTime? DeliveredAtUtc { get; init; }
}

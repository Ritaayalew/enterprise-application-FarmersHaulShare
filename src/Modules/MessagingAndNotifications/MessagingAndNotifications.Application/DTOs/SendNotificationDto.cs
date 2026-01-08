namespace MessagingAndNotifications.Application.DTOs;

/// <summary>
/// DTO for sending a notification
/// </summary>
public sealed record SendNotificationDto
{
    public required Guid RecipientId { get; init; }
    public required string RecipientType { get; init; }
    public required string ChannelType { get; init; } // "SMS", "Email", "Push", "InApp"
    public string? ChannelAddress { get; init; } // phone number, email, push token
    public required string TemplateName { get; init; }
    public required string NotificationType { get; init; } // "Quote", "StatusUpdate", "Receipt", etc.
    public Guid? RelatedEntityId { get; init; }
    public string? RelatedEntityType { get; init; }
    public Dictionary<string, string>? Metadata { get; init; } // Template placeholders
}

namespace MessagingAndNotifications.Domain.ValueObjects;

/// <summary>
/// Value object representing the channel through which a notification is sent
/// </summary>
public sealed class NotificationChannel : SharedKernel.Domain.ValueObject
{
    public string ChannelType { get; private init; } = string.Empty;
    public string? Address { get; private init; } // e.g., phone number, email, push token

    private NotificationChannel() { }

    public NotificationChannel(string channelType, string? address = null)
    {
        if (string.IsNullOrWhiteSpace(channelType))
            throw new SharedKernel.Domain.DomainException("Channel type cannot be empty.");

        var validChannels = new[] { "SMS", "Email", "Push", "InApp" };
        if (!validChannels.Contains(channelType, StringComparer.OrdinalIgnoreCase))
            throw new SharedKernel.Domain.DomainException($"Invalid channel type: {channelType}. Valid types are: {string.Join(", ", validChannels)}");

        ChannelType = channelType.Trim();
        Address = address?.Trim();
    }

    public static NotificationChannel Sms(string phoneNumber) => new("SMS", phoneNumber);
    public static NotificationChannel Email(string emailAddress) => new("Email", emailAddress);
    public static NotificationChannel Push(string pushToken) => new("Push", pushToken);
    public static NotificationChannel InApp() => new("InApp");

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return ChannelType;
        yield return Address ?? string.Empty;
    }

    public override string ToString() => Address != null ? $"{ChannelType}:{Address}" : ChannelType;
}

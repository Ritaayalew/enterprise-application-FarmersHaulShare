namespace MessagingAndNotifications.Domain.ValueObjects;

/// <summary>
/// Value object representing the status of a notification
/// </summary>
public sealed class NotificationStatus : SharedKernel.Domain.ValueObject
{
    public string Status { get; private init; } = string.Empty;
    public DateTime? StatusChangedAtUtc { get; private init; }
    public string? FailureReason { get; private init; }

    private NotificationStatus() { }

    public NotificationStatus(string status, DateTime? statusChangedAtUtc = null, string? failureReason = null)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new SharedKernel.Domain.DomainException("Status cannot be empty.");

        var validStatuses = new[] { "Pending", "Sent", "Delivered", "Failed", "Cancelled" };
        if (!validStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
            throw new SharedKernel.Domain.DomainException($"Invalid status: {status}. Valid statuses are: {string.Join(", ", validStatuses)}");

        Status = status.Trim();
        StatusChangedAtUtc = statusChangedAtUtc ?? DateTime.UtcNow;
        FailureReason = failureReason?.Trim();
    }

    public static NotificationStatus Pending() => new("Pending");
    public static NotificationStatus Sent(DateTime? atUtc = null) => new("Sent", atUtc);
    public static NotificationStatus Delivered(DateTime? atUtc = null) => new("Delivered", atUtc);
    public static NotificationStatus Failed(string reason, DateTime? atUtc = null) => new("Failed", atUtc, reason);
    public static NotificationStatus Cancelled(DateTime? atUtc = null) => new("Cancelled", atUtc);

    public bool IsPending => Status.Equals("Pending", StringComparison.OrdinalIgnoreCase);
    public bool IsSent => Status.Equals("Sent", StringComparison.OrdinalIgnoreCase);
    public bool IsDelivered => Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase);
    public bool IsFailed => Status.Equals("Failed", StringComparison.OrdinalIgnoreCase);
    public bool IsCancelled => Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Status;
        yield return StatusChangedAtUtc ?? default(DateTime);
        yield return FailureReason ?? string.Empty;
    }

    public override string ToString() => Status;
}

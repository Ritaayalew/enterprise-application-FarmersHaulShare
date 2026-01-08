using MessagingAndNotifications.Application.DTOs;

namespace MessagingAndNotifications.Application.Services;

/// <summary>
/// Service interface for notification operations
/// </summary>
public interface INotificationService
{
    Task<NotificationDto> SendNotificationAsync(SendNotificationDto dto, CancellationToken cancellationToken = default);
    Task<NotificationDto?> GetNotificationByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationDto>> GetNotificationsByRecipientAsync(Guid recipientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationDto>> GetNotificationsByTypeAsync(string notificationType, CancellationToken cancellationToken = default);
    Task MarkNotificationAsSentAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task MarkNotificationAsDeliveredAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task MarkNotificationAsFailedAsync(Guid notificationId, string reason, CancellationToken cancellationToken = default);
}

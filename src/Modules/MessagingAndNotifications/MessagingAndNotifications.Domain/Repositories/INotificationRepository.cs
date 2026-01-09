using MessagingAndNotifications.Domain.Entities;

namespace MessagingAndNotifications.Domain.Repositories;

/// <summary>
/// Repository interface for Notification entity
/// </summary>
public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetByRecipientIdAsync(Guid recipientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetByNotificationTypeAsync(string notificationType, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetByRelatedEntityAsync(Guid relatedEntityId, string? relatedEntityType = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetPendingNotificationsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

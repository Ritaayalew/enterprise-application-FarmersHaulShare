using MessagingAndNotifications.Domain.Entities;
using MessagingAndNotifications.Domain.Repositories;
using MessagingAndNotifications.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MessagingAndNotifications.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of INotificationRepository
/// </summary>
public sealed class NotificationRepository : INotificationRepository
{
    private readonly MessagingDbContext _dbContext;

    public NotificationRepository(MessagingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Notification>> GetByRecipientIdAsync(Guid recipientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .Where(n => n.RecipientId == recipientId)
            .OrderByDescending(n => n.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Notification>> GetByNotificationTypeAsync(string notificationType, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .Where(n => n.NotificationType == notificationType)
            .OrderByDescending(n => n.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Notification>> GetByRelatedEntityAsync(Guid relatedEntityId, string? relatedEntityType = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Notifications
            .Where(n => n.RelatedEntityId == relatedEntityId);

        if (!string.IsNullOrWhiteSpace(relatedEntityType))
        {
            query = query.Where(n => n.RelatedEntityType == relatedEntityType);
        }

        return await query
            .OrderByDescending(n => n.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Notification>> GetPendingNotificationsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Notifications
            .Where(n => n.Status.Status == "Pending")
            .OrderBy(n => n.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await _dbContext.Notifications.AddAsync(notification, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        _dbContext.Notifications.Update(notification);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await GetByIdAsync(id, cancellationToken);
        if (notification != null)
        {
            _dbContext.Notifications.Remove(notification);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

using MessagingAndNotifications.Application.DTOs;
using MessagingAndNotifications.Domain.Entities;
using MessagingAndNotifications.Domain.Repositories;
using MessagingAndNotifications.Domain.ValueObjects;
using MessagingAndNotifications.Domain.DomainEvents;
using SharedKernel.Domain;

namespace MessagingAndNotifications.Application.Services;

/// <summary>
/// Service implementation for notification operations
/// </summary>
public sealed class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ITemplateService _templateService;

    public NotificationService(
        INotificationRepository notificationRepository,
        ITemplateService templateService)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
    }

    public async Task<NotificationDto> SendNotificationAsync(SendNotificationDto dto, CancellationToken cancellationToken = default)
    {
        // Get or create template
        var template = await _templateService.GetTemplateAsync(dto.TemplateName, cancellationToken);
        if (template == null)
            throw new DomainException($"Template '{dto.TemplateName}' not found.");

        // Create channel
        var channel = dto.ChannelType.ToUpperInvariant() switch
        {
            "SMS" => NotificationChannel.Sms(dto.ChannelAddress ?? throw new ArgumentException("Channel address required for SMS")),
            "EMAIL" => NotificationChannel.Email(dto.ChannelAddress ?? throw new ArgumentException("Channel address required for Email")),
            "PUSH" => NotificationChannel.Push(dto.ChannelAddress ?? throw new ArgumentException("Channel address required for Push")),
            "INAPP" => NotificationChannel.InApp(),
            _ => throw new ArgumentException($"Unsupported channel type: {dto.ChannelType}")
        };

        // Create notification
        var notification = new Notification(
            id: Guid.NewGuid(),
            recipientId: dto.RecipientId,
            recipientType: dto.RecipientType,
            channel: channel,
            template: template,
            notificationType: dto.NotificationType,
            relatedEntityId: dto.RelatedEntityId,
            relatedEntityType: dto.RelatedEntityType,
            metadata: dto.Metadata);

        await _notificationRepository.AddAsync(notification, cancellationToken);

        // Map to DTO
        return MapToDto(notification);
    }

    public async Task<NotificationDto?> GetNotificationByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        return notification != null ? MapToDto(notification) : null;
    }

    public async Task<IEnumerable<NotificationDto>> GetNotificationsByRecipientAsync(Guid recipientId, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationRepository.GetByRecipientIdAsync(recipientId, cancellationToken);
        return notifications.Select(MapToDto);
    }

    public async Task<IEnumerable<NotificationDto>> GetNotificationsByTypeAsync(string notificationType, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationRepository.GetByNotificationTypeAsync(notificationType, cancellationToken);
        return notifications.Select(MapToDto);
    }

    public async Task MarkNotificationAsSentAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
        if (notification == null)
            throw new DomainException($"Notification with id {notificationId} not found.");

        notification.MarkAsSent();
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
    }

    public async Task MarkNotificationAsDeliveredAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
        if (notification == null)
            throw new DomainException($"Notification with id {notificationId} not found.");

        notification.MarkAsDelivered();
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
    }

    public async Task MarkNotificationAsFailedAsync(Guid notificationId, string reason, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
        if (notification == null)
            throw new DomainException($"Notification with id {notificationId} not found.");

        notification.MarkAsFailed(reason);
        await _notificationRepository.UpdateAsync(notification, cancellationToken);
    }

    private static NotificationDto MapToDto(Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            RecipientId = notification.RecipientId,
            RecipientType = notification.RecipientType,
            ChannelType = notification.Channel.ChannelType,
            ChannelAddress = notification.Channel.Address,
            Subject = notification.Subject,
            Body = notification.Body,
            Status = notification.Status.Status,
            NotificationType = notification.NotificationType,
            RelatedEntityId = notification.RelatedEntityId,
            RelatedEntityType = notification.RelatedEntityType,
            Metadata = notification.Metadata,
            CreatedAtUtc = notification.CreatedAtUtc,
            SentAtUtc = notification.SentAtUtc,
            DeliveredAtUtc = notification.DeliveredAtUtc
        };
    }
}

/// <summary>
/// Service for managing notification templates
/// </summary>
public interface ITemplateService
{
    Task<NotificationTemplate?> GetTemplateAsync(string templateName, CancellationToken cancellationToken = default);
    Task<NotificationTemplate> CreateTemplateAsync(string templateName, string subject, string body, string? languageCode = null, CancellationToken cancellationToken = default);
}

/// <summary>
/// Simple in-memory template service (can be replaced with database-backed implementation)
/// </summary>
public sealed class InMemoryTemplateService : ITemplateService
{
    private readonly Dictionary<string, NotificationTemplate> _templates = new();

    public InMemoryTemplateService()
    {
        // Initialize default templates
        InitializeDefaultTemplates();
    }

    public Task<NotificationTemplate?> GetTemplateAsync(string templateName, CancellationToken cancellationToken = default)
    {
        _templates.TryGetValue(templateName, out var template);
        return Task.FromResult(template);
    }

    public Task<NotificationTemplate> CreateTemplateAsync(string templateName, string subject, string body, string? languageCode = null, CancellationToken cancellationToken = default)
    {
        var template = new NotificationTemplate(templateName, subject, body, languageCode);
        _templates[templateName] = template;
        return Task.FromResult(template);
    }

    private void InitializeDefaultTemplates()
    {
        // Quote template
        _templates["Quote"] = new NotificationTemplate(
            "Quote",
            "New Haul Share Quote - {Amount} {Currency}",
            "Hello {RecipientName},\n\nA new haul share quote is available for your batch.\n\nQuote Amount: {Amount} {Currency}\nHaul Share ID: {HaulShareId}\n\nPlease review and accept if interested.\n\nBest regards,\nFarmersHaulShare Team");

        // Status update templates
        _templates["PickupStarted"] = new NotificationTemplate(
            "PickupStarted",
            "Pickup Started - Haul Share {HaulShareId}",
            "Hello {RecipientName},\n\nThe driver has started pickup for Haul Share {HaulShareId}.\n\nLocation: {Location}\nEstimated Arrival: {ETA}\n\nYou will be notified when pickup is completed.\n\nBest regards,\nFarmersHaulShare Team");

        _templates["PickupCompleted"] = new NotificationTemplate(
            "PickupCompleted",
            "Pickup Completed - Haul Share {HaulShareId}",
            "Hello {RecipientName},\n\nPickup has been completed for Haul Share {HaulShareId}.\n\nThe driver is now en route to the delivery location.\n\nBest regards,\nFarmersHaulShare Team");

        _templates["DeliveryStarted"] = new NotificationTemplate(
            "DeliveryStarted",
            "Delivery Started - Haul Share {HaulShareId}",
            "Hello {RecipientName},\n\nThe driver has started delivery for Haul Share {HaulShareId}.\n\nLocation: {Location}\nEstimated Arrival: {ETA}\n\nYou will be notified when delivery is completed.\n\nBest regards,\nFarmersHaulShare Team");

        _templates["DeliveryCompleted"] = new NotificationTemplate(
            "DeliveryCompleted",
            "Delivery Completed - Haul Share {HaulShareId}",
            "Hello {RecipientName},\n\nDelivery has been completed for Haul Share {HaulShareId}.\n\nYour receipt will be sent shortly.\n\nBest regards,\nFarmersHaulShare Team");

        // Receipt template
        _templates["Receipt"] = new NotificationTemplate(
            "Receipt",
            "Receipt - Haul Share {HaulShareId}",
            "Hello {RecipientName},\n\nYour receipt for Haul Share {HaulShareId}:\n\nTotal Amount: {TotalAmount} {Currency}\nYour Share: {RecipientShare} {Currency}\n\nThank you for using FarmersHaulShare!\n\nBest regards,\nFarmersHaulShare Team");
    }
}

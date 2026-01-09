using MessagingAndNotifications.Application.DTOs;
using MessagingAndNotifications.Application.Services;
using MessagingAndNotifications.Domain.DomainEvents;
using MessagingAndNotifications.Domain.Repositories;
using SharedKernel.Domain;
using FarmersHaulShare.SharedKernel.Domain;
using PricingAndFairCostSplit.Domain.Events;
using PricingAndFairCostSplit.Domain.ValueObjects;
using TransportMarketplaceAndDispatch.Domain.Events;
using TransportMarketplaceAndDispatch.Domain.Repositories;
using TransportMarketplaceAndDispatch.Domain.Aggregates;
using HaulShareCreationAndScheduling.Domain.Aggregates;
using HaulShareCreationAndScheduling.Infrastructure.Persistence;
using BatchPostingAndGrouping.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MessagingAndNotifications.Application.EventHandlers;


public abstract class NotificationEventHandler
{
    protected readonly INotificationService NotificationService;
    protected readonly INotificationRepository NotificationRepository;

    protected NotificationEventHandler(
        INotificationService notificationService,
        INotificationRepository notificationRepository)
    {
        NotificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        NotificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
    }

    protected async Task<Guid> SendNotificationAndRaiseEventAsync(
        SendNotificationDto sendDto,
        Func<Guid, IDomainEvent> createDomainEvent,
        CancellationToken cancellationToken = default)
    {
        var notificationDto = await NotificationService.SendNotificationAsync(sendDto, cancellationToken);

      
        var domainEvent = createDomainEvent(notificationDto.Id);
        
        return notificationDto.Id;
    }
}

public class QuoteEventHandler : NotificationEventHandler, IQuoteEventHandler
{
    private readonly HaulShareDbContext _haulShareDbContext;
    private readonly IFarmerProfileRepository _farmerProfileRepository;
    private readonly ILogger<QuoteEventHandler> _logger;

    public QuoteEventHandler(
        INotificationService notificationService,
        INotificationRepository notificationRepository,
        HaulShareDbContext haulShareDbContext,
        IFarmerProfileRepository farmerProfileRepository,
        ILogger<QuoteEventHandler> logger)
        : base(notificationService, notificationRepository)
    {
        _haulShareDbContext = haulShareDbContext ?? throw new ArgumentNullException(nameof(haulShareDbContext));
        _farmerProfileRepository = farmerProfileRepository ?? throw new ArgumentNullException(nameof(farmerProfileRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleFixedPriceQuoteCalculatedAsync(PriceCalculated priceCalculatedEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling PriceCalculated event for HaulShare {HaulShareId}", priceCalculatedEvent.HaulShareId);

        // Get haul share to find farmers
        var haulShare = await _haulShareDbContext.HaulShares
            .Include(h => h.PickupStops)
            .FirstOrDefaultAsync(h => h.Id == priceCalculatedEvent.HaulShareId, cancellationToken);

        if (haulShare == null)
        {
            _logger.LogWarning("HaulShare {HaulShareId} not found for PriceCalculated event", priceCalculatedEvent.HaulShareId);
            return;
        }

        // Get unique farmer IDs from pickup stops
        var farmerIds = haulShare.PickupStops.Select(s => s.FarmerId).Distinct().ToList();

        // Send quote notifications to each farmer
        foreach (var farmerId in farmerIds)
        {
            try
            {
                var farmer = await _farmerProfileRepository.GetByIdAsync(farmerId, cancellationToken);
                if (farmer == null)
                {
                    _logger.LogWarning("Farmer {FarmerId} not found for quote notification", farmerId);
                    continue;
                }

                var sendDto = new SendNotificationDto
                {
                    RecipientId = farmerId,
                    RecipientType = "Farmer",
                    ChannelType = "SMS", // Default to SMS, can be enhanced with user preferences
                    ChannelAddress = farmer.PhoneNumber,
                    TemplateName = "Quote",
                    NotificationType = "Quote",
                    RelatedEntityId = priceCalculatedEvent.HaulShareId,
                    RelatedEntityType = "HaulShare",
                    Metadata = new Dictionary<string, string>
                    {
                        { "Amount", priceCalculatedEvent.TotalRevenue.Amount.ToString("F2") },
                        { "Currency", priceCalculatedEvent.TotalRevenue.Currency },
                        { "PricePerKg", priceCalculatedEvent.PricePerKg.AmountPerKg.Amount.ToString("F2") },
                        { "HaulShareId", priceCalculatedEvent.HaulShareId.ToString() },
                        { "RecipientName", farmer.Name }
                    }
                };

                await SendNotificationAndRaiseEventAsync(
                    sendDto,
                    notificationId => new QuoteSent(
                        notificationId,
                        priceCalculatedEvent.HaulShareId,
                        farmerId,
                        "Farmer",
                        priceCalculatedEvent.TotalRevenue.Amount,
                        priceCalculatedEvent.TotalRevenue.Currency),
                    cancellationToken);

                _logger.LogInformation("Sent quote notification to farmer {FarmerId}", farmerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending quote notification to farmer {FarmerId}", farmerId);
                // Continue with other farmers even if one fails
            }
        }
    }
}


public class StatusUpdateEventHandler : NotificationEventHandler, IStatusUpdateEventHandler
{
    private readonly IDispatchJobRepository _dispatchJobRepository;
    private readonly HaulShareDbContext _haulShareDbContext;
    private readonly IFarmerProfileRepository _farmerProfileRepository;
    private readonly ILogger<StatusUpdateEventHandler> _logger;

    public StatusUpdateEventHandler(
        INotificationService notificationService,
        INotificationRepository notificationRepository,
        IDispatchJobRepository dispatchJobRepository,
        HaulShareDbContext haulShareDbContext,
        IFarmerProfileRepository farmerProfileRepository,
        ILogger<StatusUpdateEventHandler> logger)
        : base(notificationService, notificationRepository)
    {
        _dispatchJobRepository = dispatchJobRepository ?? throw new ArgumentNullException(nameof(dispatchJobRepository));
        _haulShareDbContext = haulShareDbContext ?? throw new ArgumentNullException(nameof(haulShareDbContext));
        _farmerProfileRepository = farmerProfileRepository ?? throw new ArgumentNullException(nameof(farmerProfileRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private async Task SendStatusUpdateNotificationsAsync(
        Guid dispatchJobId,
        string statusType,
        string? location = null,
        DateTime? estimatedTimeOfArrival = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending {StatusType} notifications for DispatchJob {DispatchJobId}", statusType, dispatchJobId);

        // Get dispatch job to find haul share
        var dispatchJob = await _dispatchJobRepository.GetByIdAsync(dispatchJobId, cancellationToken);
        if (dispatchJob == null)
        {
            _logger.LogWarning("DispatchJob {DispatchJobId} not found for status update", dispatchJobId);
            return;
        }

        // Get haul share to find farmers
        var haulShare = await _haulShareDbContext.HaulShares
            .Include(h => h.PickupStops)
            .FirstOrDefaultAsync(h => h.Id == dispatchJob.HaulShareId, cancellationToken);

        if (haulShare == null)
        {
            _logger.LogWarning("HaulShare {HaulShareId} not found for status update", dispatchJob.HaulShareId);
            return;
        }

        // Get unique farmer IDs from pickup stops
        var farmerIds = haulShare.PickupStops.Select(s => s.FarmerId).Distinct().ToList();

        // Send notifications to farmers
        foreach (var farmerId in farmerIds)
        {
            try
            {
                var farmer = await _farmerProfileRepository.GetByIdAsync(farmerId, cancellationToken);
                if (farmer == null)
                {
                    _logger.LogWarning("Farmer {FarmerId} not found for status update notification", farmerId);
                    continue;
                }

                var metadata = new Dictionary<string, string>
                {
                    { "HaulShareId", dispatchJob.HaulShareId.ToString() },
                    { "RecipientName", farmer.Name }
                };

                if (!string.IsNullOrWhiteSpace(location))
                    metadata["Location"] = location;

                if (estimatedTimeOfArrival.HasValue)
                    metadata["ETA"] = estimatedTimeOfArrival.Value.ToString("yyyy-MM-dd HH:mm");

                var sendDto = new SendNotificationDto
                {
                    RecipientId = farmerId,
                    RecipientType = "Farmer",
                    ChannelType = "SMS",
                    ChannelAddress = farmer.PhoneNumber,
                    TemplateName = statusType,
                    NotificationType = "StatusUpdate",
                    RelatedEntityId = dispatchJob.HaulShareId,
                    RelatedEntityType = "HaulShare",
                    Metadata = metadata
                };

                await SendNotificationAndRaiseEventAsync(
                    sendDto,
                    notificationId => new StatusUpdateSent(
                        notificationId,
                        dispatchJob.HaulShareId,
                        farmerId,
                        "Farmer",
                        statusType,
                        location,
                        estimatedTimeOfArrival),
                    cancellationToken);

                _logger.LogInformation("Sent {StatusType} notification to farmer {FarmerId}", statusType, farmerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending {StatusType} notification to farmer {FarmerId}", statusType, farmerId);
                // Continue with other farmers even if one fails
            }
        }

        // Send notification to driver if assigned
        if (dispatchJob.AssignedDriverId.HasValue)
        {
            try
            {
                var driverMetadata = new Dictionary<string, string>
                {
                    { "HaulShareId", dispatchJob.HaulShareId.ToString() },
                    { "DispatchJobId", dispatchJobId.ToString() }
                };

                if (!string.IsNullOrWhiteSpace(location))
                    driverMetadata["Location"] = location;

                var driverSendDto = new SendNotificationDto
                {
                    RecipientId = dispatchJob.AssignedDriverId.Value,
                    RecipientType = "Driver",
                    ChannelType = "SMS",
                    ChannelAddress = null, // Would need driver contact info from Driver repository
                    TemplateName = statusType,
                    NotificationType = "StatusUpdate",
                    RelatedEntityId = dispatchJob.HaulShareId,
                    RelatedEntityType = "HaulShare",
                    Metadata = driverMetadata
                };

                await SendNotificationAndRaiseEventAsync(
                    driverSendDto,
                    notificationId => new StatusUpdateSent(
                        notificationId,
                        dispatchJob.HaulShareId,
                        dispatchJob.AssignedDriverId.Value,
                        "Driver",
                        statusType,
                        location,
                        estimatedTimeOfArrival),
                    cancellationToken);

                _logger.LogInformation("Sent {StatusType} notification to driver {DriverId}", statusType, dispatchJob.AssignedDriverId.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending {StatusType} notification to driver {DriverId}", statusType, dispatchJob.AssignedDriverId.Value);
            }
        }
    }

    public async Task HandlePickupStartedAsync(PickupStarted pickupStartedEvent, CancellationToken cancellationToken = default)
    {
        var dispatchJob = await _dispatchJobRepository.GetByIdAsync(pickupStartedEvent.DispatchJobId, cancellationToken);
        var location = dispatchJob?.CurrentLocation != null
            ? $"{dispatchJob.CurrentLocation.Latitude}, {dispatchJob.CurrentLocation.Longitude}"
            : null;

        await SendStatusUpdateNotificationsAsync(
            pickupStartedEvent.DispatchJobId,
            "PickupStarted",
            location,
            dispatchJob?.EstimatedDeliveryTime,
            cancellationToken);
    }

    public async Task HandlePickupCompletedAsync(PickupCompleted pickupCompletedEvent, CancellationToken cancellationToken = default)
    {
        await SendStatusUpdateNotificationsAsync(
            pickupCompletedEvent.DispatchJobId,
            "PickupCompleted",
            null,
            null,
            cancellationToken);
    }

    public async Task HandleDeliveryStartedAsync(DeliveryStarted deliveryStartedEvent, CancellationToken cancellationToken = default)
    {
        var dispatchJob = await _dispatchJobRepository.GetByIdAsync(deliveryStartedEvent.DispatchJobId, cancellationToken);
        var location = dispatchJob?.CurrentLocation != null
            ? $"{dispatchJob.CurrentLocation.Latitude}, {dispatchJob.CurrentLocation.Longitude}"
            : null;

        await SendStatusUpdateNotificationsAsync(
            deliveryStartedEvent.DispatchJobId,
            "DeliveryStarted",
            location,
            dispatchJob?.EstimatedDeliveryTime,
            cancellationToken);
    }

    public async Task HandleDeliveryCompletedAsync(DeliveryCompleted deliveryCompletedEvent, CancellationToken cancellationToken = default)
    {
        await SendStatusUpdateNotificationsAsync(
            deliveryCompletedEvent.DispatchJobId,
            "DeliveryCompleted",
            null,
            null,
            cancellationToken);
    }
}

/// <summary>
/// Implementation for receipt event handler
/// </summary>
public class ReceiptEventHandler : NotificationEventHandler, IReceiptEventHandler
{
    private readonly IFarmerProfileRepository _farmerProfileRepository;
    private readonly ILogger<ReceiptEventHandler> _logger;

    public ReceiptEventHandler(
        INotificationService notificationService,
        INotificationRepository notificationRepository,
        IFarmerProfileRepository farmerProfileRepository,
        ILogger<ReceiptEventHandler> logger)
        : base(notificationService, notificationRepository)
    {
        _farmerProfileRepository = farmerProfileRepository ?? throw new ArgumentNullException(nameof(farmerProfileRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task HandleTransparencyReceiptGeneratedAsync(FairCostSplitDetermined fairCostSplitEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling FairCostSplitDetermined event for HaulShare {HaulShareId} with {FarmerCount} farmers",
            fairCostSplitEvent.HaulShareId, fairCostSplitEvent.FarmerShares.Count);

        // Send receipt notification to each farmer in the cost split
        foreach (var farmerShare in fairCostSplitEvent.FarmerShares)
        {
            try
            {
                var farmer = await _farmerProfileRepository.GetByIdAsync(farmerShare.FarmerId, cancellationToken);
                if (farmer == null)
                {
                    _logger.LogWarning("Farmer {FarmerId} not found for receipt notification", farmerShare.FarmerId);
                    continue;
                }

                var sendDto = new SendNotificationDto
                {
                    RecipientId = farmerShare.FarmerId,
                    RecipientType = "Farmer",
                    ChannelType = "SMS",
                    ChannelAddress = farmer.PhoneNumber,
                    TemplateName = "Receipt",
                    NotificationType = "Receipt",
                    RelatedEntityId = fairCostSplitEvent.HaulShareId,
                    RelatedEntityType = "HaulShare",
                    Metadata = new Dictionary<string, string>
                    {
                        { "TotalAmount", fairCostSplitEvent.TotalCost.Amount.ToString("F2") },
                        { "RecipientShare", farmerShare.ShareAmount.Amount.ToString("F2") },
                        { "Percentage", farmerShare.Percentage.ToString("F2") },
                        { "Currency", fairCostSplitEvent.TotalCost.Currency },
                        { "HaulShareId", fairCostSplitEvent.HaulShareId.ToString() },
                        { "RecipientName", farmer.Name }
                    }
                };

                await SendNotificationAndRaiseEventAsync(
                    sendDto,
                    notificationId => new ReceiptSent(
                        notificationId,
                        fairCostSplitEvent.HaulShareId,
                        farmerShare.FarmerId,
                        "Farmer",
                        fairCostSplitEvent.TotalCost.Amount,
                        farmerShare.ShareAmount.Amount,
                        fairCostSplitEvent.TotalCost.Currency),
                    cancellationToken);

                _logger.LogInformation("Sent receipt notification to farmer {FarmerId} for share {ShareAmount}",
                    farmerShare.FarmerId, farmerShare.ShareAmount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending receipt notification to farmer {FarmerId}", farmerShare.FarmerId);
                // Continue with other farmers even if one fails
            }
        }
    }
}

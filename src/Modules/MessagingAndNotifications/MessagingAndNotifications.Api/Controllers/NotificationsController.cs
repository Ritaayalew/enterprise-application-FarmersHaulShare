using MessagingAndNotifications.Application.DTOs;
using MessagingAndNotifications.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessagingAndNotifications.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

   
    [HttpPost]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NotificationDto>> SendNotification(
        [FromBody] SendNotificationDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var notification = await _notificationService.SendNotificationAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetNotification), new { id = notification.Id }, notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification");
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationDto>> GetNotification(
        Guid id,
        CancellationToken cancellationToken)
    {
        var notification = await _notificationService.GetNotificationByIdAsync(id, cancellationToken);
        if (notification == null)
            return NotFound();

        return Ok(notification);
    }

   
    [HttpGet("recipient/{recipientId}")]
    [ProducesResponseType(typeof(IEnumerable<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotificationsByRecipient(
        Guid recipientId,
        CancellationToken cancellationToken)
    {
        var notifications = await _notificationService.GetNotificationsByRecipientAsync(recipientId, cancellationToken);
        return Ok(notifications);
    }

    [HttpGet("type/{notificationType}")]
    [ProducesResponseType(typeof(IEnumerable<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotificationsByType(
        string notificationType,
        CancellationToken cancellationToken)
    {
        var notifications = await _notificationService.GetNotificationsByTypeAsync(notificationType, cancellationToken);
        return Ok(notifications);
    }

  
    [HttpPost("{id}/mark-sent")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsSent(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _notificationService.MarkNotificationAsSentAsync(id, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as sent");
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("{id}/mark-delivered")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsDelivered(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _notificationService.MarkNotificationAsDeliveredAsync(id, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as delivered");
            return NotFound(new { error = ex.Message });
        }
    }


    [HttpPost("{id}/mark-failed")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsFailed(
        Guid id,
        [FromBody] string reason,
        CancellationToken cancellationToken)
    {
        try
        {
            await _notificationService.MarkNotificationAsFailedAsync(id, reason, cancellationToken);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as failed");
            return NotFound(new { error = ex.Message });
        }
    }
}

namespace MessagingAndNotifications.Application.EventHandlers;


public interface IReceiptEventHandler
{
 
    Task HandleTransparencyReceiptGeneratedAsync(object transparencyReceiptGeneratedEvent, CancellationToken cancellationToken = default);
}

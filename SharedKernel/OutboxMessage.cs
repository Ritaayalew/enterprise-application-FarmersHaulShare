using System;
using System.Text.Json;

namespace FarmersHaulShare.SharedKernel;



public class OutboxMessage
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string EventType { get; private set; } = string.Empty;

    public string Payload { get; private set; } = string.Empty;

    public DateTime OccurredOn { get; private set; } = DateTime.UtcNow;

    public DateTime? ProcessedOn { get; private set; }

    public OutboxMessageStatus Status { get; private set; } = OutboxMessageStatus.Pending;

    public int RetryCount { get; private set; } = 0;

    public enum OutboxMessageStatus
    {
        Pending,
        Published,
        Failed
    }

    private OutboxMessage() { } // For EF

    public OutboxMessage(object domainEvent)
    {
        if (domainEvent == null) throw new ArgumentNullException(nameof(domainEvent));

        var type = domainEvent.GetType();
        EventType = $"{type.FullName}, {type.Assembly.GetName().Name}";
        Payload = JsonSerializer.Serialize(domainEvent, type);
        OccurredOn = DateTime.UtcNow;
    }

    public void MarkAsPublished()
    {
        ProcessedOn = DateTime.UtcNow;
        Status = OutboxMessageStatus.Published;
    }

    public void MarkAsFailed()
    {
        RetryCount++;
        Status = OutboxMessageStatus.Failed;
    }
}